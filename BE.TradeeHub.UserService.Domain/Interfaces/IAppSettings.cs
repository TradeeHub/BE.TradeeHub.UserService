namespace BE.TradeeHub.UserService.Domain.Interfaces;

public interface IAppSettings
{
    public string MongoDbConnectionString { get; set; }
    public string MongoDbDatabaseName { get; set; }
}