using Amazon;
using BE.TradeeHub.UserService.Domain.Interfaces;

namespace BE.TradeeHub.UserService;

public class AppSettings : IAppSettings
{
    public string Environment { get; set; }
    public string AppClientId { get; set; }
    public string UserPoolId { get; set; }
    public RegionEndpoint AWSRegion { get; set; }
    public string MongoDbConnectionString { get; set; }
    public string MongoDbDatabaseName { get; set; }
    public string? AwsAccessKeyId { get; set; }
    public string? AwsSecretAccessKey { get; set; }
    public string[] AllowedDomains { get; set; }
    public string ValidIssuer { get; set; }
    
    public AppSettings(IConfiguration config)
    {
        Environment = config["ASPNETCORE_ENVIRONMENT"];
        AwsAccessKeyId = config["AWS_ACCESS_KEY_ID"];
        AwsSecretAccessKey = config["AWS_SECRET_ACCESS_KEY"];
        AllowedDomains = config.GetSection("AppSettings:AllowedOrigins").Get<string[]>();
        MongoDbConnectionString = config.GetSection("AppSettings:MongoDB:ConnectionString").Value;
        MongoDbDatabaseName = config.GetSection("AppSettings:MongoDB:DatabaseName").Value;
        AppClientId = config.GetSection("AppSettings:Cognito:AppClientId").Value;
        UserPoolId = config.GetSection("AppSettings:Cognito:UserPoolId").Value;
        AWSRegion = RegionEndpoint.GetBySystemName(config.GetSection("AppSettings:Cognito:AWSRegion").Value);
        ValidIssuer = $"https://cognito-idp.{AWSRegion.SystemName}.amazonaws.com/{UserPoolId}";
        ValidateSettings();
    }
    
    private void ValidateSettings()
    {
        if (string.IsNullOrEmpty(AppClientId))
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.AppClientId");
        }

        if (string.IsNullOrEmpty(UserPoolId))
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.UserPoolId");
        }
        
        if (string.IsNullOrEmpty(AwsAccessKeyId))
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.AwsAccessKeyId");
        }

        if (string.IsNullOrEmpty(AwsSecretAccessKey))
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.AwsSecretAccessKey");
        }


        if (AWSRegion == null)
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.AWSRegion");
        }

        if (string.IsNullOrEmpty(MongoDbConnectionString))
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.MongoDbConnectionString");
        }

        if (string.IsNullOrEmpty(MongoDbDatabaseName))
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.MongoDbDatabaseName");
        }
        
        if (string.IsNullOrEmpty(ValidIssuer))
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.ValidIssuer");
        }
        
        if (AllowedDomains.Length == 0)
        {
            throw new ApplicationException("Missing required configuration value: AppSettings.AllowedDomains");
        }
    }
}