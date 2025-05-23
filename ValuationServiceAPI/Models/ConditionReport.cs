using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class ConditionReport
{
    [BsonId]
    public Guid ConditionReportId { get; set; } = Guid.NewGuid(); // Unique identifier for the condition report
    public string Title { get; set; } = string.Empty; // Title of the item being assessed
    public string Summary { get; set; } = string.Empty; // General overall condition summary
    public string MaterialCondition { get; set; } = string.Empty; // Condition of the materials/surface
    public string Functionality { get; set; } = string.Empty; // Operational state or working condition
    public string ComponentRemarks { get; set; } = string.Empty; // Notes about specific components or parts
    public string AuthenticityDetails { get; set; } = string.Empty; // Marks, stamps, origin, or proof
    public string Dimensions { get; set; } = string.Empty; // e.g. "Height: 23 cm, Width: 12 cm"
    public DateTime ReportDate { get; set; } = DateTime.UtcNow; // Date of the condition assessment
    public string AssessedBy { get; set; } = string.Empty; // Name or ID of the expert
    public string PdfUrl { get; set; } = string.Empty; // Hvis du vil gemme PDF-link

}
