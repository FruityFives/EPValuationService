using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ValuationServiceAPI.Models;

/// <summary>
/// Dokumentklasse til at generere en PDF tilstandsrapport ved hjælp af QuestPDF.
/// Rapporten viser detaljer om en vurderet genstand baseret på et ConditionReport-objekt.
/// </summary>
public class ConditionReportDocument : IDocument
{
    private readonly ConditionReport _report;

    /// <summary>
    /// Initialiserer en ny instans af ConditionReportDocument med en tilstandsrapport.
    /// </summary>
    /// <param name="report">Tilstandsrapport data til PDF'en.</param>
    public ConditionReportDocument(ConditionReport report)
    {
        _report = report;
    }

    /// <summary>
    /// Returnerer metadata for dokumentet.
    /// </summary>
    /// <returns>Standard dokumentmetadata.</returns>
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    /// <summary>
    /// Komponerer indholdet af PDF-dokumentet.
    /// Indeholder header med titel og dato, en indholdssektion med rapportdetaljer,
    /// samt en footer med fortrolighedsnotits.
    /// </summary>
    /// <param name="container">Dokumentcontainer til at bygge siden i.</param>
    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);

            // Header med titel og reference
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

            // Hovedindhold med vejledende tekst og rapportdetaljer
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

            // Footer med fortrolighedsnotits
            page.Footer().AlignCenter().Text("Genereret af Vurderingsservice • Fortroligt dokument")
                .FontSize(9).Italic().FontColor(Colors.Grey.Medium);
        });
    }
}
