using ValuationServiceAPI.Repository;
using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Services;

/// <summary>
/// Implementering af forretningslogik til vurderings- og rapportbehandling.
/// </summary>
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

    /// <summary>
    /// Gemmer en ny vurderingsanmodning i databasen.
    /// </summary>
    public async Task SubmitValuationRequest(ValuationRequest request)
    {
        await _repository.AddValuationRequestAsync(request);
        _logger.LogInformation("ValuationRequest gemt med ID: {Id}", request.Id);
    }

    /// <summary>
    /// Gemmer en tilstandsrapport og tilhørende vurdering, genererer PDF og publicerer til RabbitMQ.
    /// </summary>
    public async Task SubmitFullAssessmentAsync(Assessment assessment, ConditionReport report)
    {
        if (report == null) throw new ArgumentNullException(nameof(report));
        if (assessment == null) throw new ArgumentNullException(nameof(assessment));

        if (report.ConditionReportId == Guid.Empty)
        {
            report.ConditionReportId = Guid.NewGuid();
            _logger.LogInformation("Genereret ny ConditionReportId: {Id}", report.ConditionReportId);
        }

        report.PdfUrl = _pdfGenerator.GeneratePdf(report);
        await _repository.AddConditionReportAsync(report);
        _logger.LogInformation("Tilstandsrapport gemt med ID: {Id}", report.ConditionReportId);

        assessment.ConditionReportId = report.ConditionReportId;
        await _repository.AddAssessmentAsync(assessment);
        _logger.LogInformation("Assessment gemt med ID: {Id}", assessment.AssessmentId);

        var dto = new ItemAssessmentDTO
        {
            Title = assessment.Title,
            Picture = assessment.Picture,
            Category = assessment.Category,
            AssessmentPrice = assessment.AssessmentPrice,
            SellerId = assessment.ExpertId,
            ConditionReportUrl = report.PdfUrl
        };

        await _publisher.PublishAsync(dto);
        _logger.LogInformation("Assessment DTO publiceret til RabbitMQ: {Id}", assessment.AssessmentId);
    }

    /// <summary>
    /// Gemmer en tilstandsrapport uden vurdering.
    /// </summary>
    public async Task SubmitConditionReportAsync(ConditionReport report)
    {
        await _repository.AddConditionReportAsync(report);
        _logger.LogInformation("Tilstandsrapport gemt med ID: {Id}", report.ConditionReportId);
    }

    /// <summary>
    /// Opdaterer en eksisterende tilstandsrapport og regenererer PDF.
    /// </summary>
    public async Task UpdateConditionReportAsync(ConditionReport updated)
    {
        var existing = await _repository.GetConditionReportByIdAsync(updated.ConditionReportId);
        if (existing == null)
        {
            _logger.LogWarning("Ingen tilstandsrapport fundet med ID: {Id}", updated.ConditionReportId);
            throw new Exception("Tilstandsrapport ikke fundet");
        }

        // Slet tidligere PDF-fil hvis den findes
        if (!string.IsNullOrWhiteSpace(existing.PdfUrl))
        {
            var pdfPath = Path.Combine("/app/data/condition-reports", Path.GetFileName(existing.PdfUrl));
            if (File.Exists(pdfPath))
            {
                File.Delete(pdfPath);
                _logger.LogInformation("Slettede tidligere PDF: {Path}", pdfPath);
            }
        }

        // Generér ny PDF og opdater rapport
        updated.PdfUrl = _pdfGenerator.GeneratePdf(updated);
        await _repository.UpdateConditionReportAsync(updated);
        _logger.LogInformation("Tilstandsrapport og PDF opdateret for ID: {Id}", updated.ConditionReportId);
    }

    /// <summary>
    /// Opdaterer en eksisterende vurdering.
    /// </summary>
    public async Task UpdateAssessmentAsync(Assessment updated)
    {
        await _repository.UpdateAssessmentAsync(updated);
        _logger.LogInformation("Assessment opdateret for ID: {Id}", updated.AssessmentId);
    }
}
