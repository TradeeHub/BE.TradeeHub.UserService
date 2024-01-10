using BE.TradeeHub.UserService.Domain.Interfaces;
using BE.TradeeHub.UserService.Infrastructure.DbObjects;
using MongoDB.Driver;

namespace BE.TradeeHub.UserService.Infrastructure;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    
    public MongoDbContext(IAppSettings appSettings)
    {
        var client = new MongoClient(appSettings.MongoDbConnectionString);
        _database = client.GetDatabase(appSettings.MongoDbDatabaseName);
    }
    
    public IMongoCollection<UserDbObject> Users => _database.GetCollection<UserDbObject>("Users");
}