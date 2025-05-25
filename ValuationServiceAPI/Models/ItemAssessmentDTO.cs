namespace ValuationServiceAPI.Models;

/// <summary>
/// DTO sendt videre til andre services med vurderingsinformation.
/// </summary>
public class ItemAssessmentDTO
{
    public string Title { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid SellerId { get; set; }
    public decimal AssessmentPrice { get; set; }
    public string ConditionReportUrl { get; set; } = string.Empty;
}
