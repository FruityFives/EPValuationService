using ValuationServiceAPI.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ValuationServiceAPI.Models
{
    public class SubmitAssessmentDTO
    {
        public string Title { get; set; } = string.Empty;
        public decimal AssessmentPrice { get; set; }
        public Guid ExpertId { get; set; }
        public Guid ValuationRequestId { get; set; }
        public Guid SellerId { get; set; }
        public string Picture { get; set; } = string.Empty;

        public ConditionReport ConditionReport { get; set; } = new();
    }
}