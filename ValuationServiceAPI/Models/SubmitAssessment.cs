namespace ValuationServiceAPI.Models;

/// <summary>
/// Wrapper til indsendelse af både vurdering og tilstandsrapport i én request.
/// </summary>
public class SubmitAssessment
{
    public Assessment Assessment { get; set; }
    public ConditionReport ConditionReport { get; set; }
}
