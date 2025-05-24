using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ValuationServiceAPI.Models
{
    public class ConditionReport
    {
        [BsonId]
        public Guid ConditionReportId { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string MaterialCondition { get; set; } = string.Empty;
        public string Functionality { get; set; } = string.Empty;
        public string ComponentRemarks { get; set; } = string.Empty;
        public string AuthenticityDetails { get; set; } = string.Empty;
        public string Dimensions { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; } = DateTime.UtcNow;
        public string AssessedBy { get; set; } = string.Empty;
        public string PdfUrl { get; set; } = string.Empty;
    }
}
