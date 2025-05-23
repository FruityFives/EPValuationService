using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Services
{
    public interface IValuationService
    {
        Task SubmitValuationRequest(ValuationRequest request);
        Task SubmitFullAssessmentAsync(SubmitAssessmentDTO dto);
    }
}
