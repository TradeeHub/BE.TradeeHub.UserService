namespace BE.TradeeHub.UserService.Domain.Interfaces;

public interface IAppSettings
{
    public string MongoDbConnectionString { get; }
    public string MongoDbDatabaseName { get; }
    public string AppClientId { get; }
}