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

    public async Task SubmitFullAssessmentAsync(SubmitAssessmentDTO dto)
    {
        /* var report = dto.ConditionReport;

        if (report.ConditionReportId == Guid.Empty)
        {
            report.ConditionReportId = Guid.NewGuid();
            _logger.LogWarning("Generated new ConditionReportId because it was missing.");
        }*/

        dto.ConditionReport.PdfUrl = _pdfGenerator.GeneratePdf(dto.ConditionReport);

        await _repository.AddConditionReportAsync(dto.ConditionReport);
        _logger.LogInformation("ConditionReport saved with ID: {Id}", dto.ConditionReport.ConditionReportId);

        var assessment = new Assessment
        {
            Title = dto.Title,
            AssessmentPrice = dto.AssessmentPrice,
            ExpertId = dto.ExpertId,
            ValuationRequestId = dto.ValuationRequestId,
            ConditionReportId = dto.ConditionReport.ConditionReportId
        };

        await _repository.AddAssessmentAsync(assessment);
        _logger.LogInformation("Assessment saved with ID: {Id}", assessment.AssessmentId);

        var itemDto = new ItemAssessmentDTO
        {
            Title = dto.Title,
            Picture = dto.Picture,
            Category = "TODO",
            SellerId = dto.SellerId,
            AssessmentPrice = dto.AssessmentPrice,
            ConditionReportUrl = dto.ConditionReport.PdfUrl
        };

        await _publisher.PublishAsync(itemDto);
        _logger.LogInformation("Published item DTO to RabbitMQ");
    }
}
