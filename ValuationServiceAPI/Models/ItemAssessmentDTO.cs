using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Models
{
public class ItemAssessmentDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public string Category { get; set; } = "TODO";
        public string SellerId { get; set; } = string.Empty;

        public EffectAssessment Effect { get; set; } = new(); 
    }

}
