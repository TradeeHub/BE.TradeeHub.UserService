using Amazon.CognitoIdentityProvider;
using BE.TradeeHub.UserService;
using BE.TradeeHub.UserService.Mutations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

string[] allowedDomains = builder.Configuration["AppSettings:AllowedOrigins"].Split(',');
var cognitoAppClientId = builder.Configuration["AppSettings:Cognito:AppClientId"].ToString();
var cognitoUserPoolId = builder.Configuration["AppSettings:Cognito:UserPoolId"].ToString();
var cognitoAWSRegion = builder.Configuration["AppSettings:Cognito:AWSRegion"].ToString();

var validIssuer = $"https://cognito-idp.{cognitoAWSRegion}.amazonaws.com/{cognitoUserPoolId}";
var validAudience = cognitoAppClientId;

builder.Services.AddScoped<IAmazonCognitoIdentityProvider, AmazonCognitoIdentityProviderClient>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddCognitoIdentity();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins(allowedDomains)
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
    options.Authority = validIssuer;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = validIssuer,
        ValidateLifetime = true,
        ValidAudience = validAudience,
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