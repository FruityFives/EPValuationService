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
                (
                    new ValuationRequest
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
                        Description = "Vintage kamera fra 1960'erne",
                        Pictures = new List<string> { "https://example.com/kamera.jpg" }
                    },
                    new ConditionReport
                    {
                        ConditionReportId = Guid.NewGuid(),
                        Title = "Rapport: Kamera",
                        Summary = "Let brugt, virker stadig",
                        MaterialCondition = "Metal og læder, små ridser",
                        Functionality = "Fungerer som forventet",
                        ComponentRemarks = "Original linse og taske inkluderet",
                        AuthenticityDetails = "Serienummer matcher model",
                        Dimensions = "15x10x7 cm",
                        ReportDate = DateTime.UtcNow,
                        AssessedBy = "Ekspert B"
                    },
                    new Assessment
                    {
                        AssessmentId = Guid.NewGuid(),
                        Title = "Retro kamera",
                        AssessmentPrice = 2200,
                        ExpertId = Guid.NewGuid(),
                        Picture = "https://example.com/kamera.jpg",
                        Category = "Elektronik"
                    }
                ),
                (
                    new ValuationRequest
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
                        Description = "Oliemaleri af ukendt kunstner, 1923",
                        Pictures = new List<string> { "https://example.com/maleri.jpg" }
                    },
                    new ConditionReport
                    {
                        ConditionReportId = Guid.NewGuid(),
                        Title = "Rapport: Maleri",
                        Summary = "Pæn stand, original ramme",
                        MaterialCondition = "Lærred og træ",
                        Functionality = "Ikke relevant",
                        ComponentRemarks = "Ingen synlige skader",
                        AuthenticityDetails = "Ingen signatur, men aldersspor tydelige",
                        Dimensions = "70x50 cm",
                        ReportDate = DateTime.UtcNow,
                        AssessedBy = "Ekspert C"
                    },
                    new Assessment
                    {
                        AssessmentId = Guid.NewGuid(),
                        Title = "Oliemaleri 1923",
                        AssessmentPrice = 11000,
                        ExpertId = Guid.NewGuid(),
                        Picture = "https://example.com/maleri.jpg",
                        Category = "Kunst"
                    }
                )
            };

            const int maxAttempts = 10;

            foreach (var (request, report, assessment) in entries)
            {
                assessment.ValuationRequestId = request.Id;
                assessment.ConditionReportId = report.ConditionReportId;

                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        await _valuationService.SubmitValuationRequest(request);
                        await _valuationService.SubmitFullAssessmentAsync(assessment, report);
                        _logger.LogInformation("✅ Indsat: {Title}", assessment.Title);
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "❌ Fejl ved indsætning af {Title} – forsøg {Attempt}/{Max}", assessment.Title, attempt, maxAttempts);
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
