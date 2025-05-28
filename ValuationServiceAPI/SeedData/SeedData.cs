using ValuationServiceAPI.Models;
using ValuationServiceAPI.Services;

namespace ValuationServiceAPI.SeedData
{
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
            await Task.Delay(5000); // Giv RabbitMQ tid til at starte

            var isEmpty = await _valuationService.IsDatabaseEmptyAsync();
            if (!isEmpty)
            {
                _logger.LogInformation("Skipper seeding, data findes allerede i databasen.");
                return;
            }

            var entries = new List<(string Title, string Description, string Picture, string Category)>
    {
        ("Antikt vægur", "Antikt vægur fra 1800-tallet", "https://example.com/vagur.jpg", "Ure"),
        ("Retro kamera", "Vintage kamera fra 1960'erne", "https://example.com/kamera.jpg", "Elektronik"),
        ("Oliemaleri 1923", "Oliemaleri af ukendt kunstner, 1923", "https://example.com/maleri.jpg", "Kunst")
    };

            const int maxAttempts = 10;

            foreach (var (title, description, picture, category) in entries)
            {
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        var requestId = Guid.NewGuid();
                        var reportId = Guid.NewGuid();
                        var assessmentId = Guid.NewGuid();
                        var expertId = Guid.NewGuid();
                        var userId = Guid.NewGuid();

                        var request = new ValuationRequest
                        {
                            Id = requestId,
                            UserId = userId,
                            Description = description,
                            Pictures = new List<string> { picture }
                        };

                        var report = new ConditionReport
                        {
                            ConditionReportId = reportId,
                            Title = $"Rapport: {title}",
                            Summary = "Standardbeskrivelse",
                            MaterialCondition = "God stand",
                            Functionality = "Fungerer",
                            ComponentRemarks = "Ingen bemærkninger",
                            AuthenticityDetails = "Verificeret",
                            Dimensions = "Standard",
                            ReportDate = DateTime.UtcNow,
                            AssessedBy = "Ekspert"
                        };

                        var assessment = new Assessment
                        {
                            AssessmentId = assessmentId,
                            ValuationRequestId = requestId,
                            ConditionReportId = reportId,
                            Title = title,
                            AssessmentPrice = category == "Ure" ? 7500 : category == "Elektronik" ? 2200 : 11000,
                            ExpertId = expertId,
                            Picture = picture,
                            Category = category
                        };

                        await _valuationService.SubmitValuationRequest(request);
                        await _valuationService.SubmitFullAssessmentAsync(assessment, report);

                        _logger.LogInformation("✅ Indsat: {Title}", title);
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "❌ Fejl ved indsætning af {Title} – forsøg {Attempt}/{Max}", title, attempt, maxAttempts);
                        await Task.Delay(3000);
                    }
                }
            }

            await _valuationService.SubmitValuationRequest(new ValuationRequest
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Description = "Ubehandlet: Gammel skrivemaskine fra 1940’erne",
                Pictures = new List<string> { "https://example.com/skrivemaskine.jpg" }
            });

            await _valuationService.SubmitValuationRequest(new ValuationRequest
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Description = "Ubehandlet: Klassisk telefon i bakelit",
                Pictures = new List<string> { "https://example.com/telefon.jpg" }
            });

            _logger.LogInformation("✅ Indsat 2 ubehandlede vurderingsanmodninger.");
            _logger.LogInformation("🎉 Seeding afsluttet.");
        }

    }
}
