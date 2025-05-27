using Microsoft.AspNetCore.Mvc;
using ValuationServiceAPI.Models;
using ValuationServiceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace ValuationServiceAPI.Controllers
{
    /// <summary>
    /// Controller til håndtering af vurderingsrelaterede API-kald.
    /// Indeholder endpoints til at indsende vurderingsanmodninger, tilstandsrapporter og vurderingsresultater.
    /// </summary>
    [ApiController]
    [Route("api/valuation")]
    public class ValuationController : ControllerBase
    {
        private readonly IValuationService _service;
        private readonly ILogger<ValuationController> _logger;

        /// <summary>
        /// Konstruktor der injicerer service og logger.
        /// </summary>
        public ValuationController(IValuationService service, ILogger<ValuationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Modtager en vurderingsanmodning fra en autoriseret admin-bruger.
        /// Validerer token og sender anmodningen til serviceslaget.
        /// </summary>
        /// <param name="request">Vurderingsanmodning med nødvendige data.</param>
        /// <returns>200 OK ved succes, 401 Unauthorized ved manglende eller ugyldige claimss.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("valuationrequest")]
        public async Task<IActionResult> SubmitValuation([FromBody] ValuationRequest request)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("TestAuth hit. Username: {Username}, Role: {Role}", username, role);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                _logger.LogWarning("Token validation failed. Missing claims.");
                return Unauthorized("Invalid token.");
            }

            await _service.SubmitValuationRequest(request);
            return Ok("Valuation saved");
        }

        /// <summary>
        /// Modtager og gemmer en tilstandsrapport.
        /// Tildeler automatisk et nyt Guid hvis ID mangler.
        /// </summary>
        /// <param name="report">Tilstandsrapport til gemning.</param>
        /// <returns>Guid for den gemte tilstandsrapport.</returns>

        /// <summary>
        /// Modtager en fuld vurderingsrapport inklusiv vurdering og tilstandsrapport.
        /// </summary>
        /// <param name="dto">Dataoverførsel med vurdering og tilstandsrapport.</param>
        /// <returns>200 OK ved succes, 400 BadRequest hvis tilstandsrapport mangler.</returns>
        [HttpPost("addeffectassessment")]
        public async Task<IActionResult> SubmitAssessment([FromBody] SubmitAssessment dto)
        {
            _logger.LogInformation("Modtog assessment request fra bruger");
            if (dto.ConditionReport == null)
                return BadRequest("Condition report is required.");

            await _service.SubmitFullAssessmentAsync(dto.Assessment, dto.ConditionReport);
            return Ok("Assessment and report submitted");
        }

        /// <summary>
        /// Opdaterer en eksisterende tilstandsrapport.
        /// </summary>
        /// <param name="updatedReport">Opdateret tilstandsrapport.</param>
        /// <returns>200 OK ved succes.</returns>
        [HttpPut("update/conditionreport")]
        public async Task<IActionResult> UpdateConditionReport([FromBody] ConditionReport updatedReport)
        {
            await _service.UpdateConditionReportAsync(updatedReport);
            return Ok("Condition report updated.");
        }

        /// <summary>
        /// Opdaterer en eksisterende vurdering.
        /// </summary>
        /// <param name="updatedAssessment">Opdateret vurdering.</param>
        /// <returns>200 OK ved succes.</returns>
        [HttpPut("update/assessment/{id}")]
        public async Task<IActionResult> UpdateAssessment(Assessment updatedAssessment)
        {
            await _service.UpdateAssessmentAsync(updatedAssessment);
            return Ok("Assessment updated.");
        }

        /// <summary>
        /// Henter alle vurderingsanmodninger i databasen.
        /// </summary>
        /// <returns>En liste af ValuationRequest-objekter.</returns>
        [HttpGet("requests")]
        public async Task<IActionResult> GetAllValuationRequests()
        {
            try
            {
                var requests = await _service.GetAllValuationRequestsAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fejl ved hentning af vurderingsanmodninger.");
                return StatusCode(500, "Intern serverfejl");
            }
        }

    }
}
