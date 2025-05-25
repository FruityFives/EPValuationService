using System.IO;
using QuestPDF.Fluent;
using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Services;

/// <summary>
/// Genererer en PDF baseret på en tilstandsrapport og gemmer den på filsystemet.
/// </summary>
public class ConditionReportPdfGenerator : IConditionReportPdfGenerator
{
    private readonly string _outputPath = "/app/data/condition-reports";

    /// <summary>
    /// Genererer PDF for en given tilstandsrapport og returnerer stien til filen.
    /// </summary>
    /// <param name="report">Den tilstandsrapport der skal konverteres til PDF.</param>
    /// <returns>En relativ URL til den genererede PDF.</returns>
    public string GeneratePdf(ConditionReport report)
    {
        if (!Directory.Exists(_outputPath))
        {
            Directory.CreateDirectory(_outputPath);
        }

        var fileName = $"{report.ConditionReportId}.pdf";
        var fullFilePath = Path.Combine(_outputPath, fileName);

        var document = new ConditionReportDocument(report);
        document.GeneratePdf(fullFilePath);

        // Relativ sti til frontend/api for adgang
        return $"/condition-reports/{fileName}";
    }
}
