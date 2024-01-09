using BE.TradeeHub.UserService.Domain.Interfaces;
using BE.TradeeHub.UserService.Infrastructure.DbObjects;
using MongoDB.Driver;

namespace BE.TradeeHub.UserService.Infrastructure;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly IAppSettings _appSettings;
    
    public MongoDbContext()
    {
        var client = new MongoClient(_appSettings.MongoDbConnectionString);
        _database = client.GetDatabase(_appSettings.MongoDbDatabaseName);
    }
    
    public IMongoCollection<UserDbObject> Customers => _database.GetCollection<UserDbObject>("Users");
}