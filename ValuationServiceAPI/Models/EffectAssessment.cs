using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ValuationServiceAPI.Models
{
    public class EffectAssessment
    {
        public string Title { get; set; } = string.Empty;
       
        public decimal AssessmentPrice { get; set; }
        // public ConditionReport ConditionReport { get; set; } = new();
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [BsonRepresentation(BsonType.String)]
        public Guid ExpertId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ValuationRequestId { get; set; }


    }
}
