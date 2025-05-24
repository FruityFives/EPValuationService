using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ValuationServiceAPI.Models
{
    public class ItemAssessmentDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public Guid SellerId { get; set; }
        public decimal AssessmentPrice { get; set; }
        public string ConditionReportUrl { get; set; } = string.Empty;

    }
}
