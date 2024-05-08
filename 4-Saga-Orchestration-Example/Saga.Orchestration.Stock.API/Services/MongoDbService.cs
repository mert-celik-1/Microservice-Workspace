using MongoDB.Driver;

namespace Saga.Orchestration.Stock.API.Services;

public class MongoDbService
{
    readonly IMongoDatabase _database;
    public MongoDbService(IConfiguration configuration)
    {
        MongoClient client = new(configuration.GetConnectionString("MongoDB"));
        _database = client.GetDatabase("StockDB");
    }
    public IMongoCollection<T> GetCollection<T>() => _database.GetCollection<T>(typeof(T).Name.ToLowerInvariant());
}