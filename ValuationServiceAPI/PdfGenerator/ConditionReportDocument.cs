using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ValuationServiceAPI.Models;

/// <summary>
/// Genererer en PDF-rapport baseret på en tilstandsrapport (ConditionReport).
/// Rapporten bruges som vejledende vurdering til brugere uden fysisk adgang til varen.
/// </summary>
public class ConditionReportDocument : IDocument
{
    private readonly ConditionReport _report;

    /// <summary>
    /// Initialiserer en ny instans af <see cref="ConditionReportDocument"/> med en given tilstandsrapport.
    /// </summary>
    /// <param name="report">Tilstandsrapporten der skal konverteres til PDF.</param>
    public ConditionReportDocument(ConditionReport report)
    {
        _report = report;
    }

    /// <summary>
    /// Returnerer metadata for dokumentet (standardopsætning).
    /// </summary>
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    /// <summary>
    /// Komponerer indholdet i PDF-dokumentet, herunder header, rapportdata og footer.
    /// </summary>
    /// <param name="container">Dokumentcontaineren hvor indholdet bygges.</param>
    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);

            // Sidehoved
            page.Header().Row(row =>
            {
                row.RelativeColumn().Text("Tilstandsrapport")
                    .FontSize(24).Bold().FontColor(Colors.Black);

                row.ConstantColumn(200).Column(col =>
                {
                    col.Item().AlignRight().Text("Vurderingsservice").Bold();
                    col.Item().AlignRight().Text($"Dato: {DateTime.UtcNow:dd.MM.yyyy}");
                    col.Item().AlignRight().Text("Ref: VS-001");
                });
            });

            // Rapportindhold
            page.Content().PaddingVertical(20).Column(col =>
            {
                col.Item().Text(text =>
                {
                    text.Span("Denne rapport er udarbejdet som en service for brugere, der ikke har mulighed for at inspicere varen fysisk. ")
                        .Italic().FontSize(10);
                    text.Span("Bemærk, at rapporten kun er vejledende og ikke kan erstatte en personlig vurdering.")
                        .Italic().FontSize(10).FontColor(Colors.Grey.Medium);
                });

                col.Item().PaddingTop(15).Border(1).Padding(15).Column(box =>
                {
                    box.Item().Text($"Titel: {_report.Title}").Bold();
                    box.Item().Text($"Sammendrag: {_report.Summary}");
                    box.Item().Text($"Materiale: {_report.MaterialCondition}");
                    box.Item().Text($"Funktionalitet: {_report.Functionality}");
                    box.Item().Text($"Bemærkninger: {_report.ComponentRemarks}");
                    box.Item().Text($"Autenticitet: {_report.AuthenticityDetails}");
                    box.Item().Text($"Mål: {_report.Dimensions}");
                    box.Item().Text($"Vurderet af: {_report.AssessedBy}");
                    box.Item().Text($"Rapportdato: {_report.ReportDate:dd-MM-yyyy}");
                });
            });

            // Sidefod
            page.Footer().AlignCenter().Text("Genereret af Vurderingsservice • Fortroligt dokument")
                .FontSize(9).Italic().FontColor(Colors.Grey.Medium);
        });
    }
}
