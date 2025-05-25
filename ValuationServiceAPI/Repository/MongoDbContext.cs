using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Repository;

/// <summary>
/// Konfigurerer forbindelse til MongoDB og eksponerer samlinger (collections).
/// </summary>
public class MongoDbContext
{
    /// <summary>Reference til MongoDB-databasen.</summary>
    public IMongoDatabase Database { get; }

    /// <summary>Collection med vurderingsanmodninger.</summary>
    public IMongoCollection<ValuationRequest> ValuationRequests { get; }

    /// <summary>Collection med vurderinger (effect assessments).</summary>
    public IMongoCollection<Assessment> Assessments { get; }

    /// <summary>Collection med tilstandsrapporter.</summary>
    public IMongoCollection<ConditionReport> ConditionReports { get; }

    /// <summary>
    /// Initialiserer forbindelse til MongoDB og samlinger baseret på miljøvariabler eller fallback-værdier.
    /// </summary>
    /// <param name="logger">Logger til informationsmeddelelser.</param>
    /// <param name="config">Konfiguration med miljøvariabler.</param>
    public MongoDbContext(ILogger<MongoDbContext> logger, IConfiguration config)
    {
        var connectionString = config["MONGODB_URI"] ?? "mongodb://mongo:27017";
        var dbName = config["VALUATION_DB_NAME"] ?? "ValuationDB";

        logger.LogInformation("Oprettet forbindelse til MongoDB database: {Db}", dbName);

        // Sikrer at GUID'er serialiseres som string i databasen
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        var client = new MongoClient(connectionString);
        Database = client.GetDatabase(dbName);

        // Initialisering af collections
        ValuationRequests = Database.GetCollection<ValuationRequest>("ValuationRequests");
        Assessments = Database.GetCollection<Assessment>("EffectAssessments");
        ConditionReports = Database.GetCollection<ConditionReport>("ConditionReports");
    }
}
