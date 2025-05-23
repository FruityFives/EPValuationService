using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Repository
{
    public interface IValuationRepository
    {
        Task AddValuationRequestAsync(ValuationRequest request);
        Task AddAssessmentAsync(Assessment assessment);
        Task AddConditionReportAsync(ConditionReport report);
    }
}
