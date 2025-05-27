using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Services
{
    public interface IConditionReportPdfGenerator
    {
        string GeneratePdf(ConditionReport report);
    }
}