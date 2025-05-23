using Microsoft.AspNetCore.Mvc;
using ValuationServiceAPI.Models;
using ValuationServiceAPI.Services;


namespace ValuationServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuationController : ControllerBase
    {
        private readonly IValuationService _service;
        private readonly ILogger<ValuationController> _logger;

        public ValuationController(IValuationService service, ILogger<ValuationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("✅ ValuationService is up");

        [HttpPost("valuation-request")]
        public async Task<IActionResult> SubmitValuation([FromBody] ValuationRequest request)
        {
            _logger.LogInformation("📩 Modtog valuation request fra bruger {userId}", request.UserId);
            await _service.SubmitValuationRequest(request);
            return Ok("Valuation saved");
        }

        [HttpPost("effect-assessment")]
        public async Task<IActionResult> SubmitAssessment([FromBody] Assessment assessment)
        {
            _logger.LogInformation("Received assessment for '{Title}'", assessment.Title);
            await _service.SubmitFullAssessmentAsync(assessment);
            return Ok("Assessment submitted successfully");
        }

    }
}

