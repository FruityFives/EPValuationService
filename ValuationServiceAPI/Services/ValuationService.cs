using ValuationServiceAPI.Repository;
using ValuationServiceAPI.Models;
using System.Xml;

namespace ValuationServiceAPI.Services;

public class ValuationService : IValuationService
{
    private readonly ILogger<ValuationService> _logger;
    private readonly IRabbitMqPublisher _publisher;
    private readonly IConditionReportPdfGenerator _pdfGenerator;
    private readonly IValuationRepository _repository;

    public ValuationService(
        ILogger<ValuationService> logger,
        IRabbitMqPublisher publisher,
        IConditionReportPdfGenerator pdfGenerator,
        IValuationRepository repository)
    {
        _logger = logger;
        _publisher = publisher;
        _pdfGenerator = pdfGenerator;
        _repository = repository;
    }

    public async Task SubmitValuationRequest(ValuationRequest request)
    {
        await _repository.AddValuationRequestAsync(request);
        _logger.LogInformation("ValuationRequest saved with ID: {Id}", request.Id);
    }

    public async Task SubmitFullAssessmentAsync(Assessment assessment, ConditionReport report)
    {
        if (report == null) throw new ArgumentNullException(nameof(report));
        if (assessment == null) throw new ArgumentNullException(nameof(assessment));

        if (report.ConditionReportId == Guid.Empty)
        {
            report.ConditionReportId = Guid.NewGuid();
            _logger.LogInformation("Generated new ConditionReportId: {Id}", report.ConditionReportId);
        }

        report.PdfUrl = _pdfGenerator.GeneratePdf(report);

        await _repository.AddConditionReportAsync(report);
        _logger.LogInformation("Saved ConditionReport with ID: {Id}", report.ConditionReportId);

        assessment.ConditionReportId = report.ConditionReportId;
        await _repository.AddAssessmentAsync(assessment);
        _logger.LogInformation("Saved Assessment with ID: {Id}", assessment.AssessmentId);

        var dto = new ItemAssessmentDTO
        {
            Title = assessment.Title,
            Picture = assessment.Picture,
            Category = assessment.Category,
            AssessmentPrice = assessment.AssessmentPrice,
            SellerId = assessment.ExpertId,
            ConditionReportUrl = report.PdfUrl,
        };

        await _publisher.PublishAsync(dto);
        _logger.LogInformation("Published item DTO to RabbitMQ for Assessment: {Id}", assessment.AssessmentId);
    }

    public async Task SubmitConditionReportAsync(ConditionReport report)
    {
        await _repository.AddConditionReportAsync(report);
        _logger.LogInformation("Inserted ConditionReport with ID: {Id}", report.ConditionReportId);
    }

    public async Task UpdateConditionReportAsync(ConditionReport updated)
    {
        // Hent eksisterende rapport fra database
        var existing = await _repository.GetConditionReportByIdAsync(updated.ConditionReportId);
        if (existing == null)
        {
            _logger.LogWarning("ConditionReport not found with ID: {Id}", updated.ConditionReportId);
            throw new Exception("ConditionReport not found");
        }

        // Slet gammel PDF hvis den findes
        if (!string.IsNullOrWhiteSpace(existing.PdfUrl))
        {
            var pdfPath = Path.Combine("/app/data/condition-reports", Path.GetFileName(existing.PdfUrl));
            if (File.Exists(pdfPath))
            {
                File.Delete(pdfPath);
                _logger.LogInformation("Deleted old PDF file: {Path}", pdfPath);
            }
        }

        // Generer ny PDF og opdater URL
        updated.PdfUrl = _pdfGenerator.GeneratePdf(updated);

        // Opdater i databasen
        await _repository.UpdateConditionReportAsync(updated);
        _logger.LogInformation("ConditionReport updated and PDF regenerated for ID: {Id}", updated.ConditionReportId);
    }


    public async Task UpdateAssessmentAsync(Assessment updated)
    {
        await _repository.UpdateAssessmentAsync(updated);
        _logger.LogInformation("Assessment updated for ID: {Id}", updated.AssessmentId);
    }


}
