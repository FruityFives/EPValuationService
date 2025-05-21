namespace ValuationServiceAPI.Models
{
    public class ItemAssessmentDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public string Category { get; set; } = "TODO";
        public Guid SellerId { get; set; }

        public decimal AssessmentPrice { get; set; }
      

      
    }
}
