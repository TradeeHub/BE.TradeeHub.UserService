using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using BE.TradeeHub.UserService.Domain.Interfaces;
using BE.TradeeHub.UserService.Domain.Interfaces.Repositories;
using BE.TradeeHub.UserService.Infrastructure.DbObjects;
using BE.TradeeHub.UserService.Infrastructure.Repository;
using BE.TradeeHub.UserService.Requests;
using BE.TradeeHub.UserService.Responses;

namespace BE.TradeeHub.UserService;

public class AuthService
{
    private readonly IAmazonCognitoIdentityProvider _cognitoService;
    private readonly IAppSettings _appSettings;
    private readonly UserRepository _userRepository;
    public AuthService(IAmazonCognitoIdentityProvider cognitoService, IAppSettings appSettings, UserRepository userRepository)
    {
        _cognitoService = cognitoService;
        _appSettings = appSettings;
        _userRepository = userRepository;
    }

    public async Task<ConfirmSignUpResponse> ConfirmRegistrationAsync(string confirmationCode, string email, CancellationToken ctx)
    {
        var request = new ConfirmSignUpRequest
        {
            ClientId = _appSettings.AppClientId,
            Username = email,
            ConfirmationCode = confirmationCode
        };

        var confirmationResponse =  await _cognitoService.ConfirmSignUpAsync(request, ctx);
        
        if (confirmationResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            // If the confirmation is successful, update the user in MongoDB
            await _userRepository.UpdateUserEmailVerifiedStatus(email, true, ctx);
        }

        return confirmationResponse;
    }

    public async Task<ResendConfirmationCodeResponse> ResendConfirmationCodeAsync(string email, CancellationToken ctx)
    {
        var request = new ResendConfirmationCodeRequest
        {
            ClientId = _appSettings.AppClientId,
            Username = email
        };
        
        var response = await _cognitoService.ResendConfirmationCodeAsync(request, ctx);

        return response;
    }
    
        public async Task<UserDbObject> AddRandomUser(RegisterRequest request, CancellationToken ctx)
    {
        
        var user = new UserDbObject
        {
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Name = request.Name,
            CompanyName = request.CompanyName,
            CompanyType = request.CompanyType,
            Address = request.Address,
            GeneralInfo = new GeneralCompanyInfoDbObject
            {
                MarketingPreference = request.MarketingPreference,
                AnnualRevenue = request.AnnualRevenue,
                CompanyPriority = request.CompanyPriority,
                CompanySize = request.CompanySize
            },
            CreatedDate = DateTime.UtcNow,
        };

        await _userRepository.AddUserAsync(user, ctx);

        return user;
    } 
    
    public async Task<SignUpResponse> RegisterAsync(RegisterRequest request, CancellationToken ctx)
    {
        var signUpRequest = new SignUpRequest
        {
            ClientId = _appSettings.AppClientId,
            Username = request.Email,
            Password = request.Password,
            UserAttributes = new List<AttributeType>
            {
                new AttributeType
                {
                    Name = "name",
                    Value = request.Name
                },
                new AttributeType
                {
                    Name = "custom:company_name",
                    Value = request.CompanyName
                },
                new AttributeType
                {
                    Name = "custom:annual_revenue",
                    Value = request.AnnualRevenue
                },
                new AttributeType
                {
                    Name = "custom:company_size",
                    Value = request.CompanySize
                },
                new AttributeType
                {
                    Name = "custom:company_priority",
                    Value = request.CompanyPriority
                },
                new AttributeType
                {
                    Name = "custom:company_type",
                    Value = request.CompanyType
                },
                new AttributeType
                {
                    Name = "custom:marketing_preference",
                    Value = request.MarketingPreference
                },
                // The 'address' attribute seems to be a required, not custom attribute, and should not have the 'custom:' prefix
                new AttributeType
                {
                    Name = "address",
                    Value = request.Address
                },
                // The 'phone_number' attribute is also a standard attribute and should be named as 'phone_number', not 'phone'
                new AttributeType
                {
                    Name = "phone_number",
                    Value = request.PhoneNumber
                }
                // Add other attributes here if necessary
            }
        };

        var response = await _cognitoService.SignUpAsync(signUpRequest, ctx);

        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK) return response;
        
        var user = new UserDbObject
        {
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Name = request.Name,
            CompanyName = request.CompanyName,
            CompanyType = request.CompanyType,
            Address = request.Address,
            GeneralInfo = new GeneralCompanyInfoDbObject
            {
                MarketingPreference = request.MarketingPreference,
                AnnualRevenue = request.AnnualRevenue,
                CompanyPriority = request.CompanyPriority,
                CompanySize = request.CompanySize
            },
            CreatedDate = DateTime.UtcNow,
        };

        await _userRepository.AddUserAsync(user, ctx);

        return response;
    }
    
    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var authRequest = new InitiateAuthRequest
        {
            ClientId = _appSettings.AppClientId,
            AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
            AuthParameters = new Dictionary<string, string>
            {
                { "USERNAME", request.Username },
                { "PASSWORD", request.Password }
            }
        };

        var authResponse = await _cognitoService.InitiateAuthAsync(authRequest);
        
        return new TokenResponse
        {
            IdToken = authResponse.AuthenticationResult.IdToken,
            AccessToken = authResponse.AuthenticationResult.AccessToken,
            RefreshToken = authResponse.AuthenticationResult.RefreshToken
        };
    }
}