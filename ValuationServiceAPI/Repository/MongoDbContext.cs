using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Repository
{
    public class MongoDbContext
    {
        public IMongoDatabase Database { get; }
        public IMongoCollection<ValuationRequest> ValuationRequests { get; }
        public IMongoCollection<Assessment> Assessments { get; }
        public IMongoCollection<ConditionReport> ConditionReports { get; }

        public MongoDbContext(ILogger<MongoDbContext> logger, IConfiguration config)
        {
            // MongoDB environment-based config
            var connectionString = config["MONGODB_URI"] ?? "mongodb://mongo:27017";
            var dbName = config["VALUATION_DB_NAME"] ?? "ValuationDB";

            logger.LogInformation("✅ Connected to database {Db}", dbName);

            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            var client = new MongoClient(connectionString);
            Database = client.GetDatabase(dbName);

            // Initialize collections
            ValuationRequests = Database.GetCollection<ValuationRequest>("ValuationRequests");
            Assessments = Database.GetCollection<Assessment>("EffectAssessments");
            ConditionReports = Database.GetCollection<ConditionReport>("ConditionReports");
        }
    }
}
