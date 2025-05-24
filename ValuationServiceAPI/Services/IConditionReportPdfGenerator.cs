using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Services
{
    public interface IConditionReportPdfGenerator
    {
        /// <summary>
        /// Generates a PDF from a condition report and returns the relative URL path.
        /// </summary>
        string GeneratePdf(ConditionReport report);
    }
}