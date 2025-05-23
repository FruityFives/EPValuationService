using MongoDB.Driver;
using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Repository
{
    public class ValuationRepository : IValuationRepository
    {
        private readonly IMongoCollection<ValuationRequest> _valuationCollection;
        private readonly IMongoCollection<Assessment> _assessmentCollection;
        private readonly IMongoCollection<ConditionReport> _conditionReportCollection;
        private readonly ILogger<ValuationRepository> _logger;

        public ValuationRepository(
            ILogger<ValuationRepository> logger,
            MongoDbContext context)
        {
            _logger = logger;
            _valuationCollection = context.ValuationRequests;
            _assessmentCollection = context.Assessments;
            _conditionReportCollection = context.ConditionReports;
        }

        public async Task AddValuationRequestAsync(ValuationRequest request)
        {
            try
            {
                await _valuationCollection.InsertOneAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting ValuationRequest");
                throw;
            }
        }

        public async Task AddAssessmentAsync(Assessment assessment)
        {
            try
            {
                await _assessmentCollection.InsertOneAsync(assessment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting Assessment");
                throw;
            }
        }

        public async Task AddConditionReportAsync(ConditionReport report)
        {
            try
            {
                await _conditionReportCollection.InsertOneAsync(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting ConditionReport");
                throw;
            }
        }
    }
}
