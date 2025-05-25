using MongoDB.Driver;
using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Repository;

/// <summary>
/// Repository til databaseoperationer relateret til vurderinger og tilstandsrapporter.
/// </summary>
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

    /// <summary>
    /// Tilføjer en ny vurderingsanmodning til databasen.
    /// </summary>
    public async Task AddValuationRequestAsync(ValuationRequest request)
    {
        try
        {
            await _valuationCollection.InsertOneAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fejl ved indsættelse af ValuationRequest");
            throw;
        }
    }

    /// <summary>
    /// Tilføjer en ny vurdering (Assessment) til databasen.
    /// </summary>
    public async Task AddAssessmentAsync(Assessment assessment)
    {
        try
        {
            await _assessmentCollection.InsertOneAsync(assessment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fejl ved indsættelse af Assessment");
            throw;
        }
    }

    /// <summary>
    /// Tilføjer en ny tilstandsrapport til databasen.
    /// </summary>
    public async Task AddConditionReportAsync(ConditionReport report)
    {
        try
        {
            await _conditionReportCollection.InsertOneAsync(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fejl ved indsættelse af ConditionReport");
            throw;
        }
    }

    /// <summary>
    /// Opdaterer en eksisterende tilstandsrapport i databasen.
    /// </summary>
    public async Task UpdateConditionReportAsync(ConditionReport updated)
    {
        var filter = Builders<ConditionReport>.Filter.Eq(r => r.ConditionReportId, updated.ConditionReportId);

        try
        {
            await _conditionReportCollection.ReplaceOneAsync(filter, updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fejl ved opdatering af ConditionReport med ID {Id}", updated.ConditionReportId);
            throw;
        }
    }

    /// <summary>
    /// Opdaterer en eksisterende vurdering i databasen.
    /// </summary>
    public async Task UpdateAssessmentAsync(Assessment updated)
    {
        var filter = Builders<Assessment>.Filter.Eq(a => a.AssessmentId, updated.AssessmentId);

        try
        {
            await _assessmentCollection.ReplaceOneAsync(filter, updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fejl ved opdatering af Assessment med ID {Id}", updated.AssessmentId);
            throw;
        }
    }

    /// <summary>
    /// Henter en tilstandsrapport baseret på ID.
    /// </summary>
    /// <param name="conditionReportId">ID på tilstandsrapporten.</param>
    /// <returns>Den fundne ConditionReport eller null.</returns>
    public async Task<ConditionReport?> GetConditionReportByIdAsync(Guid conditionReportId)
    {
        try
        {
            var filter = Builders<ConditionReport>.Filter.Eq(r => r.ConditionReportId, conditionReportId);
            return await _conditionReportCollection.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fejl ved hentning af ConditionReport med ID {Id}", conditionReportId);
            throw;
        }
    }
}
