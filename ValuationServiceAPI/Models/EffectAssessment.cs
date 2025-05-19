namespace ValuationServiceAPI.Models
{
    public class EffectAssessment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public decimal AssessmentPrice { get; set; }
        public ConditionReport ConditionReport { get; set; } = new();
        public string ExpertId { get; set; } = string.Empty;
        public string EffectId { get; set; } = string.Empty;
    }
}
