using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using BE.TradeeHub.UserService;
using BE.TradeeHub.UserService.Domain.Entities;
using BE.TradeeHub.UserService.Domain.Interfaces;
using BE.TradeeHub.UserService.Domain.Interfaces.Repositories;
using BE.TradeeHub.UserService.GraphQL.DataLoaders;
using BE.TradeeHub.UserService.GraphQL.Mutations;
using BE.TradeeHub.UserService.GraphQL.Queries;
using BE.TradeeHub.UserService.GraphQL.QueryResolvers;
using BE.TradeeHub.UserService.GraphQL.Types;
using BE.TradeeHub.UserService.Infrastructure;
using BE.TradeeHub.UserService.Infrastructure.Repository;
using BE.TradeeHub.UserService.Interfaces;
using BE.TradeeHub.UserService.Services;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var appSettings = new AppSettings(builder.Configuration);
builder.Services.AddSingleton<IAppSettings>(appSettings);
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("GraphQLCorsPolicy",
        builder =>
        {
            builder.WithOrigins(appSettings.AllowedDomains)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddCognitoIdentity();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<StaffDataLoader>();
builder.Services.AddScoped<CompaniesMemberOfDataLoader>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TypeResolver>();
builder.Services.AddScoped<UserContext>();
builder.Services.AddSingleton<IMongoCollection<UserEntity>>(serviceProvider =>
{
    var mongoDbContext = serviceProvider.GetRequiredService<MongoDbContext>();
    return mongoDbContext.Users; // Assuming this is the property name in MongoDbContext for the collection
});

// If I am on dev the setting should come from bottom right of rider aws toolkit no need to pass any values
if (appSettings.Environment.Contains("dev", StringComparison.CurrentCultureIgnoreCase))
{
    builder.Services.AddScoped<IAmazonCognitoIdentityProvider, AmazonCognitoIdentityProviderClient>();
}
else
{
    //this means I am in docker and values are not saved anywhere in the solution but only in my docker environment variable you can edit the docker in rider ide(not the file)
    var awsOptions = builder.Configuration.GetAWSOptions();

    awsOptions.Credentials = new BasicAWSCredentials(
        appSettings.AwsAccessKeyId, 
        appSettings.AwsSecretAccessKey
    );
    builder.Services.AddSingleton<IAmazonCognitoIdentityProvider>(sp =>
        new AmazonCognitoIdentityProviderClient(awsOptions.Credentials, appSettings.AWSRegion)
    );
}

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = appSettings.ValidIssuer;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = appSettings.ValidIssuer,
        ValidateLifetime = true,
        ValidAudience = appSettings.AppClientId,
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
    };
    
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("jwt"))
            {
                context.Token = context.Request.Cookies["jwt"];
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services
    .AddGraphQLServer()
    // .AddApolloFederation()
    .AddGlobalObjectIdentification()
    .AddAuthorization()
    .AddDataLoader<StaffDataLoader>()
    .AddDataLoader<CompaniesMemberOfDataLoader>()
    .BindRuntimeType<ObjectId, IdType>()
    .AddTypeConverter<ObjectId, string>(o => o.ToString())
    .AddTypeConverter<string, ObjectId>(o => ObjectId.Parse(o))
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<UserType>()
    .AddMongoDbSorting()
    .AddMongoDbProjections()
    .AddMongoDbPagingProviders()
    .AddMongoDbFiltering();

var app = builder.Build();


if (appSettings.Environment is "Development" or "Docker")
{
    var resolver = app.Services.GetService<IRequestExecutorResolver>();
    var executor = resolver?.GetRequestExecutorAsync().Result;
    if (executor != null)
    {
        const string schemaFile = "schema.graphql";
        var newSchema = executor.Schema.ToString();
        var oldSchema = File.ReadAllText(schemaFile);
        if (newSchema != oldSchema)
            File.WriteAllText(schemaFile, newSchema);
    }
}

app.UseCors("GraphQLCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();
app.MapGraphQL();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/graphql/", permanent: false);
    }
    else
    {
        await next();
    }
});

app.Run();

