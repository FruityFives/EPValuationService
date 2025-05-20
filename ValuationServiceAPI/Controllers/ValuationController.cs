using Microsoft.AspNetCore.Mvc;
using ValuationServiceAPI.Models;
using ValuationServiceAPI.Services;


namespace ValuationServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuationController : ControllerBase
    {
   private readonly ValuationService _service;
    private readonly ILogger<ValuationController> _logger;

public ValuationController(ValuationService service, ILogger<ValuationController> logger)
{
    _service = service;     
    _logger = logger;
}



       [HttpPost("valuation-request")]
public async Task<IActionResult> SubmitValuation([FromBody] ValuationRequest request)
{
    _logger.LogInformation("📩 Modtog valuation request fra bruger {userId}", request.UserId);
    await _service.SubmitValuationRequest(request);
    return Ok("Valuation saved");
}

[HttpPost("effect-assessment")]
public async Task<IActionResult> SubmitAssessment([FromBody] EffectAssessment assessment)
{
    _logger.LogInformation("📝 Modtog effect assessment med titel '{title}'", assessment.Id);
    await _service.SendEffectAssessmentAsync(assessment);
    return Ok("Assessment sent");
}
    }
}
