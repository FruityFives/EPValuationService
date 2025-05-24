using System.IO;
using QuestPDF.Fluent;
using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Services
{
    public class ConditionReportPdfGenerator : IConditionReportPdfGenerator
    {
        // Folder where PDFs will be stored – must match volume mount
        private readonly string _outputPath = "/app/data/condition-reports";

        public string GeneratePdf(ConditionReport report)
        {
            // Ensure the output directory exists
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }

            var fileName = $"{report.ConditionReportId}.pdf";
            var fullFilePath = Path.Combine(_outputPath, fileName);

            // Use QuestPDF layout class
            var document = new ConditionReportDocument(report);
            document.GeneratePdf(fullFilePath); // QuestPDF call

            // Return the relative URL for the API or frontend
            return $"/files/condition-reports/{fileName}";
        }
    }
}
