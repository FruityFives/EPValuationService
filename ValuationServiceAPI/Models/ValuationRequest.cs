using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ValuationServiceAPI.Models
{
    public class ValuationRequest
    {
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        public string Description { get; set; } = string.Empty;
        public List<string> Pictures { get; set; } = new();
       
    }
}
