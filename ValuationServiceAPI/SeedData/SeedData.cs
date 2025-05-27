using ValuationServiceAPI.Models;
using ValuationServiceAPI.Services;

namespace ValuationServiceAPI.SeedData;

public class SeedData
{
    private readonly IValuationService _valuationService;
    private readonly ILogger<SeedData> _logger;

    public SeedData(IValuationService valuationService, ILogger<SeedData> logger)
    {
        _valuationService = valuationService;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starter seeding af eksempeldata...");

        var entries = new List<(ValuationRequest, ConditionReport, Assessment)>
    {
        (
            new ValuationRequest
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Description = "Antikt vægur fra 1800-tallet",
                Pictures = new List<string> { "https://example.com/vagur.jpg" }
            },
            new ConditionReport
            {
                ConditionReportId = Guid.NewGuid(),
                Title = "Rapport: Vægur",
                Summary = "Meget velholdt, original urværk",
                MaterialCondition = "Træ og messing",
                Functionality = "Fungerer korrekt",
                ComponentRemarks = "Små ridser på overflade",
                AuthenticityDetails = "Indgraveret initialer, verificeret ægte",
                Dimensions = "60x30x10 cm",
                ReportDate = DateTime.UtcNow,
                AssessedBy = "Ekspert A"
            },
            new Assessment
            {
                AssessmentId = Guid.NewGuid(),
                Title = "Antikt vægur",
                AssessmentPrice = 7500,
                ExpertId = Guid.NewGuid(),
                Picture = "https://example.com/vagur.jpg",
                Category = "Ure"
            }
        ),
        // ... tilføj flere eksempler her, som du allerede har
    };

        const int maxAttempts = 10;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                foreach (var (request, report, assessment) in entries)
                {
                    assessment.ValuationRequestId = request.Id;
                    assessment.ConditionReportId = report.ConditionReportId;

                    await _valuationService.SubmitValuationRequest(request);
                    await _valuationService.SubmitFullAssessmentAsync(assessment, report);
                    _logger.LogInformation("✔ Indsat: {Title}", assessment.Title);
                }

                _logger.LogInformation("✅ Seeding afsluttet.");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Forsøg {Attempt}/{Max} mislykkedes. Prøver igen om 3 sekunder...", attempt, maxAttempts);
                await Task.Delay(3000);
            }
        }

        _logger.LogError("❌ Kunne ikke færdiggøre seeding efter {Max} forsøg.", maxAttempts);
    }
}
