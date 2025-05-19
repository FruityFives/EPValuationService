namespace ValuationServiceAPI.Models
{
    public class ValuationRequest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Description { get; set; } = string.Empty;
        public List<string> Pictures { get; set; } = new();
        public string UserId { get; set; } = string.Empty;
    }
}
