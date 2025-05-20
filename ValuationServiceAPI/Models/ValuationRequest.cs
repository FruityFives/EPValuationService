namespace ValuationServiceAPI.Models
{
    public class ValuationRequest
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Pictures { get; set; } = new();
        public Guid UserId { get; set; } 
    }
}
