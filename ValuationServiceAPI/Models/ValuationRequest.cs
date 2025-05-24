using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ValuationServiceAPI.Models
{
    public class ValuationRequest
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Pictures { get; set; } = new();
    }
}