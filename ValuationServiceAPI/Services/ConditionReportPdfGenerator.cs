using System.IO;
using QuestPDF.Fluent;
using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Services
{
    /// <summary>
    /// Genererer PDF-filer baseret på ConditionReport-objekter og gemmer dem i et specificeret output-bibliotek.
    /// </summary>
    public class ConditionReportPdfGenerator : IConditionReportPdfGenerator
    {
        private readonly string _outputPath = "/app/data/condition-reports";

        /// <summary>
        /// Genererer en PDF for den givne tilstandsrapport og returnerer URL'en til PDF-filen.
        /// </summary>
        /// <param name="report">Tilstandsrapporten, der skal genereres PDF for.</param>
        /// <returns>Relativ URL til den genererede PDF-fil.</returns>
        public string GeneratePdf(ConditionReport report)
        {
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }

            var fileName = $"{report.ConditionReportId}.pdf";
            var fullFilePath = Path.Combine(_outputPath, fileName);

            var document = new ConditionReportDocument(report);
            document.GeneratePdf(fullFilePath); // QuestPDF call

            return $"/condition-reports/{fileName}";
        }
    }
}