using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ValuationServiceAPI.Models
{
    public class Assessment
    {
        [BsonId]
        public Guid AssessmentId { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public decimal AssessmentPrice { get; set; }
        public Guid ExpertId { get; set; }
        public Guid ValuationRequestId { get; set; }
        public Guid ConditionReportId { get; set; }
        public string Picture { get; set; }
        public string Category { get; set; }
    }
}