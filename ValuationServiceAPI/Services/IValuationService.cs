using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Services
{
    public interface IValuationService
    {
        Task SubmitValuationRequest(ValuationRequest request);
        Task SubmitFullAssessmentAsync(Assessment assessment, ConditionReport report);
        Task UpdateConditionReportAsync(ConditionReport updated);
        Task UpdateAssessmentAsync(Assessment updated);
        Task<bool> IsDatabaseEmptyAsync();
        Task<IEnumerable<ValuationRequest>> GetAllValuationRequestsAsync();

    }
}
