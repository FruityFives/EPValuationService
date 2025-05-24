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

        private readonly IConditionReportPdfGenerator _pdfGenerator;

        public ValuationController(IValuationService service, ILogger<ValuationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("ValuationService is up");

        [HttpPost("valuationrequest")]
        public async Task<IActionResult> SubmitValuation([FromBody] ValuationRequest request)
        {
            _logger.LogInformation("Modtog valuation request fra bruger {userId}", request.UserId);
            await _service.SubmitValuationRequest(request);
            return Ok("Valuation saved");
        }

        [HttpPost("addconditionreport")]
        public async Task<IActionResult> SubmitConditionReport([FromBody] ConditionReport report)
        {
            if (report.ConditionReportId == Guid.Empty)
                report.ConditionReportId = Guid.NewGuid();

            await _service.SubmitConditionReportAsync(report);

            _logger.LogInformation("Saved condition report with ID {Id}", report.ConditionReportId);
            return Ok(report.ConditionReportId);
        }


        [HttpPost("addeffectassessment")]
        public async Task<IActionResult> SubmitAssessment([FromBody] SubmitAssessment dto)
        {
            _logger.LogInformation("Modtog assessment request fra bruger");
            if (dto.ConditionReport == null)
                return BadRequest("Condition report is required.");

            await _service.SubmitFullAssessmentAsync(dto.Assessment, dto.ConditionReport);
            return Ok("Assessment and report submitted");
        }

        [HttpPut("update/conditionreport")]
        public async Task<IActionResult> UpdateConditionReport([FromBody] ConditionReport updatedReport)
        {
            await _service.UpdateConditionReportAsync(updatedReport);
            return Ok("Condition report updated.");
        }

        [HttpPut("update/assessment/{id}")]
        public async Task<IActionResult> UpdateAssessment(Assessment updatedAssessment)
        {
            await _service.UpdateAssessmentAsync(updatedAssessment);
            return Ok("Assessment updated.");
        }

    }
}

