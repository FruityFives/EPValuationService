namespace ValuationServiceAPI.Models
{
    public class EffectAssessment
    {
        public string Title { get; set; } = string.Empty;
        public Guid Id { get; set; } 
        public decimal AssessmentPrice { get; set; }
       // public ConditionReport ConditionReport { get; set; } = new();
        public Guid ExpertId { get; set; }
        public Guid EffectId { get; set; } 
    }
}
