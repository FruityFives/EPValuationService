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


        public async Task<ValuationRequest?> GetRequestByIdAsync(Guid id)
        {
            return await _valuationCollection.Find(r => r.Id == id).FirstOrDefaultAsync();
        }


        public async Task SendEffectAssessmentAsync(EffectAssessment assessment, ValuationRequest request)
        {
            // DTO konstrueres med data både fra EffectAssessment og ValuationRequest
            var dto = new ItemAssessmentDTO
            {
                Title = assessment.Title,
                Picture = request.Pictures.FirstOrDefault() ?? "", // Brug første billede fra ValuationRequest
                Category = "TODO",
                SellerId = request.UserId,                         // Sælgeren er brugeren der lavede valuation requesten
                AssessmentPrice = assessment.AssessmentPrice,
            };

            await _publisher.PublishAsync(dto);
            _logger.LogInformation("DTO sendt til RabbitMQ med kobling til ValuationRequestId: {Id}", request.Id);
        }


    }
}
