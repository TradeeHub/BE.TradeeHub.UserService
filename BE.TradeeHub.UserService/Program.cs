using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using BE.TradeeHub.UserService;
using BE.TradeeHub.UserService.Domain.Interfaces;
using BE.TradeeHub.UserService.Mutations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var appSettings = new AppSettings(builder.Configuration);
builder.Services.AddSingleton<IAppSettings>(appSettings);
builder.Services.AddScoped<IAmazonCognitoIdentityProvider, AmazonCognitoIdentityProviderClient>();

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

builder.Services.AddScoped<AuthService>();
builder.Services.AddCognitoIdentity();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins(appSettings.AllowedDomains)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

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
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/testAuth", () => "Hello TEST AUTH!").RequireAuthorization();
app.MapGet("/testAuth2", () => "Hello TEST 22222!").RequireAuthorization();

app.MapPost("/login", async ([FromBody] LoginRequest tokenRequest, AuthService authService) =>
{
    try
    {
        var tokenResponse = await authService.LoginAsync(tokenRequest);
        return Results.Ok(tokenResponse);
    }
    catch (Exception ex)
    {
        // Handle exceptions
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/register", async ([FromBody] RegisterRequest registrationData, AuthService authService) =>
{
    try
    {
        // Call your Cognito service to register the user
        await authService.SignUpUserAsync(registrationData);
        return Results.Ok("User registered successfully.");
    }
    catch (Exception ex)
    {
        // Handle exceptions (e.g., user already exists)
        return Results.Problem(ex.Message);
    }
});

app.Run();