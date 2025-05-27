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

        public async Task UpdateConditionReportAsync(ConditionReport updated)
        {
            var filter = Builders<ConditionReport>.Filter.Eq(r => r.ConditionReportId, updated.ConditionReportId);
            await _conditionReportCollection.ReplaceOneAsync(filter, updated);
        }

        public async Task UpdateAssessmentAsync(Assessment updated)
        {
            var filter = Builders<Assessment>.Filter.Eq(a => a.AssessmentId, updated.AssessmentId);
            await _assessmentCollection.ReplaceOneAsync(filter, updated);
        }

        public async Task<ConditionReport?> GetConditionReportByIdAsync(Guid conditionReportId)
        {
            try
            {
                var filter = Builders<ConditionReport>.Filter.Eq(r => r.ConditionReportId, conditionReportId);
                return await _conditionReportCollection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ConditionReport with ID {Id}", conditionReportId);
                throw;
            }
        }

        public async Task<bool> IsDatabaseEmptyAsync()
        {
            try
            {
                var count = await _valuationCollection.CountDocumentsAsync(FilterDefinition<ValuationRequest>.Empty);
                return count == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if database is empty");
                throw;
            }

        }

        public async Task<IEnumerable<ValuationRequest>> GetAllValuationRequestsAsync()
        {
            try
            {
                return await _valuationCollection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fejl ved hentning af alle ValuationRequests.");
                throw;
            }
        }
    }
}
