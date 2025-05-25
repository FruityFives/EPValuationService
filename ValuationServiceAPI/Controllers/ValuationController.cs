using Microsoft.AspNetCore.Mvc;
using ValuationServiceAPI.Models;
using ValuationServiceAPI.Services;

namespace ValuationServiceAPI.Controllers
{
    [ApiController]
    [Route("api/valuation")]
    public class ValuationController : ControllerBase
    {
        private readonly IValuationService _service;
        private readonly ILogger<ValuationController> _logger;

        public ValuationController(IValuationService service, ILogger<ValuationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Simpelt ping-endpoint til statuscheck af ValuationService.
        /// </summary>
        [HttpGet("ping")]
        public IActionResult Ping() => Ok("ValuationService is up");

        /// <summary>
        /// Modtager og gemmer en ny vurderingsanmodning.
        /// </summary>
        [HttpPost("valuationrequest")]
        public async Task<IActionResult> SubmitValuation([FromBody] ValuationRequest request)
        {
            _logger.LogInformation("Modtog valuation request fra bruger {UserId}", request.UserId);
            await _service.SubmitValuationRequest(request);
            return Ok("Valuation saved");
        }

        /// <summary>
        /// Modtager og gemmer en tilstandsrapport.
        /// </summary>
        [HttpPost("addconditionreport")]
        public async Task<IActionResult> SubmitConditionReport([FromBody] ConditionReport report)
        {
            if (report.ConditionReportId == Guid.Empty)
                report.ConditionReportId = Guid.NewGuid();

            await _service.SubmitConditionReportAsync(report);

            _logger.LogInformation("Tilstandsrapport gemt med ID {Id}", report.ConditionReportId);
            return Ok(report.ConditionReportId);
        }

        /// <summary>
        /// Modtager en effektvurdering og tilhørende tilstandsrapport og gemmer begge.
        /// </summary>
        [HttpPost("addeffectassessment")]
        public async Task<IActionResult> SubmitAssessment([FromBody] SubmitAssessment dto)
        {
            _logger.LogInformation("Modtog effektvurdering og tilstandsrapport");

            if (dto.ConditionReport == null)
                return BadRequest("Tilstandsrapport skal angives.");

            await _service.SubmitFullAssessmentAsync(dto.Assessment, dto.ConditionReport);
            return Ok("Assessment and report submitted");
        }

        /// <summary>
        /// Opdaterer en eksisterende tilstandsrapport.
        /// </summary>
        [HttpPut("update/conditionreport")]
        public async Task<IActionResult> UpdateConditionReport([FromBody] ConditionReport updatedReport)
        {
            await _service.UpdateConditionReportAsync(updatedReport);
            return Ok("Condition report updated.");
        }

        /// <summary>
        /// Opdaterer en eksisterende effektvurdering.
        /// </summary>
        [HttpPut("update/assessment/{id}")]
        public async Task<IActionResult> UpdateAssessment([FromBody] Assessment updatedAssessment)
        {
            await _service.UpdateAssessmentAsync(updatedAssessment);
            return Ok("Assessment updated.");
        }
    }
}
