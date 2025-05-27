using Microsoft.AspNetCore.Mvc;
using ValuationServiceAPI.Models;
using ValuationServiceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;


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
        

        [Authorize(Roles = "Admin")]
        [HttpPost("test-auth")]
        public IActionResult TestAuth()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("TestAuth hit. Username: {Username}, Role: {Role}", username, role);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                _logger.LogWarning("Token validation failed. Missing claims.");
                return Unauthorized("Invalid token.");
            }

            return Ok("Token virker!");
        }


        [Authorize (Roles = "Ekspert, Admin")]
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

        [Authorize(Roles = "Ekspert, Admin")]
        [HttpPost("addconditionreport")]
        public async Task<IActionResult> SubmitConditionReport([FromBody] ConditionReport report)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("TestAuth hit. Username: {Username}, Role: {Role}", username, role);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                _logger.LogWarning("Token validation failed. Missing claims.");
                return Unauthorized("Invalid token.");
            }
            if (report.ConditionReportId == Guid.Empty)
                report.ConditionReportId = Guid.NewGuid();

            await _service.SubmitConditionReportAsync(report);

            _logger.LogInformation("Saved condition report with ID {Id}", report.ConditionReportId);
            return Ok(report.ConditionReportId);
        }

        [Authorize(Roles = "Ekspert, Admin")]
        [HttpPost("addeffectassessment")]
        public async Task<IActionResult> SubmitAssessment([FromBody] SubmitAssessment dto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("TestAuth hit. Username: {Username}, Role: {Role}", username, role);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                _logger.LogWarning("Token validation failed. Missing claims.");
                return Unauthorized("Invalid token.");
            }
            _logger.LogInformation("Modtog assessment request fra bruger");
            if (dto.ConditionReport == null)
                return BadRequest("Condition report is required.");

            await _service.SubmitFullAssessmentAsync(dto.Assessment, dto.ConditionReport);
            return Ok("Assessment and report submitted");
        }

        [Authorize(Roles = "Ekspert, Admin")]
        [HttpPut("update/conditionreport")]
        public async Task<IActionResult> UpdateConditionReport([FromBody] ConditionReport updatedReport)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("TestAuth hit. Username: {Username}, Role: {Role}", username, role);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                _logger.LogWarning("Token validation failed. Missing claims.");
                return Unauthorized("Invalid token.");
            }
            await _service.UpdateConditionReportAsync(updatedReport);
            return Ok("Condition report updated.");
        }

        [Authorize(Roles = "Ekspert, Admin")]
        [HttpPut("update/assessment/{id}")]
        public async Task<IActionResult> UpdateAssessment(Assessment updatedAssessment)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            _logger.LogInformation("TestAuth hit. Username: {Username}, Role: {Role}", username, role);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
            {
                _logger.LogWarning("Token validation failed. Missing claims.");
                return Unauthorized("Invalid token.");
            }
            await _service.UpdateAssessmentAsync(updatedAssessment);
            return Ok("Assessment updated.");
        }

    }
}

