using MongoDB.Driver;
using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Services
{
    public class ValuationService
    {
        private readonly IMongoCollection<ValuationRequest> _valuationCollection;
        private readonly ILogger<ValuationService> _logger;
        private readonly IRabbitMqPublisher _publisher;

        public ValuationService(IMongoClient client, ILogger<ValuationService> logger, IRabbitMqPublisher publisher)
        {
            var database = client.GetDatabase("ValuationDB");
            _valuationCollection = database.GetCollection<ValuationRequest>("ValuationRequests");
            _logger = logger;
            _publisher = publisher;
        }

        public async Task SubmitValuationRequest(ValuationRequest request)
        {
            await _valuationCollection.InsertOneAsync(request);
            _logger.LogInformation("ValuationRequest gemt med ID: {Id}", request.Id);
        }

        public async Task SendEffectAssessmentAsync(EffectAssessment assessment)
{
    var dto = new ItemAssessmentDTO
    {
        Title = assessment.ConditionReport.Title,
        Picture = assessment.ConditionReport.Pictures.FirstOrDefault() ?? "",
        Category = "TODO",
        SellerId = assessment.ExpertId,
        Effect = assessment // hele objektet indlejret
    };

    await _publisher.PublishAsync(dto);
    _logger.LogInformation("DTO sendt til RabbitMQ med embedded EffectAssessment");
}

    }
}
