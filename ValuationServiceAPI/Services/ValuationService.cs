using ValuationServiceAPI.Repository;
using ValuationServiceAPI.Models;
using System.Xml;

namespace ValuationServiceAPI.Services;

public class ValuationService : IValuationService
{
    private readonly ILogger<ValuationService> _logger;
    private readonly IRabbitMqPublisher _publisher;
    private readonly IValuationRepository _repository;

    public ValuationService(
        ILogger<ValuationService> logger,
        IRabbitMqPublisher publisher,
        IValuationRepository repository)
    {
        _logger = logger;
        _publisher = publisher;
        _repository = repository;
    }

    public async Task SubmitValuationRequest(ValuationRequest request)
    {
        await _repository.AddValuationRequestAsync(request);
        _logger.LogInformation("ValuationRequest saved with ID: {Id}", request.Id);
    }

    public async Task SubmitFullAssessmentAsync(Assessment assessment)
    {
        await _repository.AddAssessmentAsync(assessment);
        _logger.LogInformation("Assessment saved with ID: {Id}", assessment.AssessmentId);

        var itemDto = new ItemAssessmentDTO
        {
            Title = assessment.Title,
            Picture = assessment., // hvis ikke relevant, fjern fra DTO
            Category = "TODO",
            SellerId = Guid.NewGuid(), // eller hent fra andet sted
            AssessmentPrice = assessment.AssessmentPrice,
            ConditionReportUrl = "" // tom eller fjernes fra DTO
        };

        await _publisher.PublishAsync(itemDto);
        _logger.LogInformation("📤 Published item DTO to RabbitMQ");
    }
}
