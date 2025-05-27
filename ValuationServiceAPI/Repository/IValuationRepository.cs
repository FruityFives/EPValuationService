using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Repository
{
    public interface IValuationRepository
    {
        Task AddValuationRequestAsync(ValuationRequest request);
        Task AddAssessmentAsync(Assessment assessment);
        Task AddConditionReportAsync(ConditionReport report);
        Task UpdateConditionReportAsync(ConditionReport report);
        Task UpdateAssessmentAsync(Assessment assessment);
        Task<ConditionReport?> GetConditionReportByIdAsync(Guid conditionReportId);
        Task<bool> IsDatabaseEmptyAsync();
        Task<IEnumerable<ValuationRequest>> GetAllValuationRequestsAsync();

    }
}
