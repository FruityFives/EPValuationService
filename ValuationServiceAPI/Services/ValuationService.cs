using ValuationServiceAPI.Repository;
using ValuationServiceAPI.Models;
using System.Xml;

namespace ValuationServiceAPI.Services
{
    /// <summary>
    /// Serviceklasse til håndtering af vurderingsprocesser, herunder lagring, opdatering og publicering.
    /// </summary>
    public class ValuationService : IValuationService
    {
        private readonly ILogger<ValuationService> _logger;
        private readonly IRabbitMqPublisher _publisher;
        private readonly IConditionReportPdfGenerator _pdfGenerator;
        private readonly IValuationRepository _repository;

        /// <summary>
        /// Initialiserer en ny instans af <see cref="ValuationService"/>.
        /// </summary>
        /// <param name="logger">Logger til informations- og fejlhåndtering.</param>
        /// <param name="publisher">Publisher til RabbitMQ-beskeder.</param>
        /// <param name="pdfGenerator">PDF-generator til tilstandsrapporter.</param>
        /// <param name="repository">Repository til databaseoperationer.</param>
        public ValuationService(
            ILogger<ValuationService> logger,
            IRabbitMqPublisher publisher,
            IConditionReportPdfGenerator pdfGenerator,
            IValuationRepository repository)
        {
            _logger = logger;
            _publisher = publisher;
            _pdfGenerator = pdfGenerator;
            _repository = repository;
        }

        /// <summary>
        /// Gemmer en ny vurderingsanmodning i databasen.
        /// </summary>
        /// <param name="request">Den vurderingsanmodning, der skal gemmes.</param>
        public async Task SubmitValuationRequest(ValuationRequest request)
        {
            await _repository.AddValuationRequestAsync(request);
            _logger.LogInformation("ValuationRequest saved with ID: {Id}", request.Id);
        }

        /// <summary>
        /// Gemmer en fuld vurdering inklusiv tilstandsrapport, genererer PDF og publicerer resultat til RabbitMQ.
        /// </summary>
        /// <param name="assessment">Vurderingsdata.</param>
        /// <param name="report">Tilstandsrapporten, der skal tilknyttes vurderingen.</param>
        /// <exception cref="ArgumentNullException">Hvis assessment eller report er null.</exception>
        public async Task SubmitFullAssessmentAsync(Assessment assessment, ConditionReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            if (assessment == null) throw new ArgumentNullException(nameof(assessment));

            if (report.ConditionReportId == Guid.Empty)
            {
                report.ConditionReportId = Guid.NewGuid();
                _logger.LogInformation("Generated new ConditionReportId: {Id}", report.ConditionReportId);
            }

            report.PdfUrl = _pdfGenerator.GeneratePdf(report);
            await _repository.AddConditionReportAsync(report);
            _logger.LogInformation("Saved ConditionReport with ID: {Id}", report.ConditionReportId);

            assessment.ConditionReportId = report.ConditionReportId;
            await _repository.AddAssessmentAsync(assessment);
            _logger.LogInformation("Saved Assessment with ID: {Id}", assessment.AssessmentId);

            var dto = new ItemAssessmentDTO
            {
                Title = assessment.Title,
                Picture = assessment.Picture,
                Category = assessment.Category,
                AssessmentPrice = assessment.AssessmentPrice,
                SellerId = assessment.ExpertId,
                ConditionReportUrl = report.PdfUrl,
            };

            await _publisher.PublishAsync(dto);
            _logger.LogInformation("Published item DTO to RabbitMQ for Assessment: {Id}", assessment.AssessmentId);
        }

        /// <summary>
        /// Gemmer en tilstandsrapport i databasen.
        /// </summary>
        /// <param name="report">Tilstandsrapporten der skal gemmes.</param>


        /// <summary>
        /// Opdaterer en eksisterende tilstandsrapport, regenererer PDF og sletter gammel PDF-fil.
        /// </summary>
        /// <param name="updated">Den opdaterede tilstandsrapport.</param>
        /// <exception cref="Exception">Hvis den eksisterende tilstandsrapport ikke findes.</exception>
        public async Task UpdateConditionReportAsync(ConditionReport updated)
        {
            var existing = await _repository.GetConditionReportByIdAsync(updated.ConditionReportId);
            if (existing == null)
            {
                _logger.LogWarning("ConditionReport not found with ID: {Id}", updated.ConditionReportId);
                throw new Exception($"ConditionReport not found with ID: {updated.ConditionReportId}");
            }

            if (!string.IsNullOrWhiteSpace(existing.PdfUrl))
            {
                var pdfPath = Path.Combine("/app/data/condition-reports", Path.GetFileName(existing.PdfUrl));
                if (File.Exists(pdfPath))
                {
                    File.Delete(pdfPath);
                    _logger.LogInformation("Deleted old PDF file: {Path}", pdfPath);
                }
            }

            updated.PdfUrl = _pdfGenerator.GeneratePdf(updated);
            await _repository.UpdateConditionReportAsync(updated);
            _logger.LogInformation("ConditionReport updated and PDF regenerated for ID: {Id}", updated.ConditionReportId);
        }

        /// <summary>
        /// Opdaterer en eksisterende vurdering i databasen.
        /// </summary>
        /// <param name="updated">Den opdaterede vurdering.</param>
        public async Task UpdateAssessmentAsync(Assessment updated)
        {
            await _repository.UpdateAssessmentAsync(updated);
            _logger.LogInformation("Assessment updated for ID: {Id}", updated.AssessmentId);
        }

        /// <summary>
        /// Undersøger om databasen er tom for vurderingsanmodninger, vurderinger og tilstandsrapporter.
        /// </summary>
        /// <returns>True hvis databasen er tom, ellers false.</returns>
        public async Task<bool> IsDatabaseEmptyAsync()
        {
            try
            {
                var isEmpty = await _repository.IsDatabaseEmptyAsync();
                _logger.LogInformation("Database empty check: {IsEmpty}", isEmpty);
                return isEmpty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if database is empty");
                throw;
            }
        }
        
        /// <summary>
        /// Henter alle vurderingsanmodninger fra databasen.
        /// /// </summary>
        /// <returns>En liste af ValuationRequest-objekter.</returns>
        public async Task<IEnumerable<ValuationRequest>> GetAllValuationRequestsAsync()
        {
            return await _repository.GetAllValuationRequestsAsync();
        }

    }
}
