using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ValuationServiceAPI.Models;

public class ConditionReportDocument : IDocument
{
    private readonly ConditionReport _report;

    public ConditionReportDocument(ConditionReport report)
    {
        _report = report;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);

            // HEADER
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

            page.Footer().AlignCenter().Text("Genereret af Vurderingsservice • Fortroligt dokument")
                .FontSize(9).Italic().FontColor(Colors.Grey.Medium);
        });
    }
}
