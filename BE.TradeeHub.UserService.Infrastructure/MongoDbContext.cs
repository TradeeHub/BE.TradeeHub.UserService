using BE.TradeeHub.UserService.Domain.Entities;
using BE.TradeeHub.UserService.Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE.TradeeHub.UserService.Infrastructure;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    
    public MongoDbContext(IAppSettings appSettings)
    {
        var settings = MongoClientSettings.FromConnectionString(appSettings.MongoDbConnectionString);
        settings.GuidRepresentation = GuidRepresentation.Standard;

        // Now create the MongoClient using the configured settings.
        var client = new MongoClient(settings);
        
        _database = client.GetDatabase(appSettings.MongoDbDatabaseName);
    }
    
    public IMongoCollection<UserEntity> Users => _database.GetCollection<UserEntity>("Users");
}