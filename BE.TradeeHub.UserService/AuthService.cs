using System.IdentityModel.Tokens.Jwt;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using BE.TradeeHub.UserService.Domain.Interfaces;
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

    public AuthService(IAmazonCognitoIdentityProvider cognitoService, IAppSettings appSettings,
        UserRepository userRepository)
    {
        _cognitoService = cognitoService;
        _appSettings = appSettings;
        _userRepository = userRepository;
    }

    public async Task<ConfirmSignUpResponse> ConfirmRegistrationAsync(string confirmationCode, string email,
        CancellationToken ctx)
    {
        var request = new ConfirmSignUpRequest
        {
            ClientId = _appSettings.AppClientId,
            Username = email,
            ConfirmationCode = confirmationCode
        };

        var confirmationResponse = await _cognitoService.ConfirmSignUpAsync(request, ctx);

        if (confirmationResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            // If the confirmation is successful, update the user in MongoDB
            await _userRepository.UpdateUserEmailVerifiedStatus(email, true, ctx);
        }

        return confirmationResponse;
    }

    public async Task<ResendConfirmationCodeResponse> ResendVerificationCodeAsync(string email, CancellationToken ctx)
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
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            Place = new PlaceDbObject()
            {
                PlaceId = request.Place.PlaceId,
                Address = request.Place.Address,
                Location = new LocationDbObject()
                {
                    Lat = request.Place.Location.Lat,
                    Lng = request.Place.Location.Lng
                },
                Viewport = new ViewPortDbObject()
                {
                    Northeast = new LocationDbObject()
                    {
                        Lat = request.Place.Viewport.Northeast.Lat,
                        Lng = request.Place.Viewport.Northeast.Lng
                    },
                    Southwest = new LocationDbObject()
                    {
                        Lat = request.Place.Viewport.Southwest.Lat,
                        Lng = request.Place.Viewport.Southwest.Lng
                    }
                }
            },
            CompanyName = request.CompanyName,
            CompanyType = request.CompanyType,
            CompanySize = request.CompanySize,
            ReferralSource = request.ReferralSource,
            CompanyPriority = request.CompanyPriority,
            MarketingPreference = request.MarketingPreference,
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
                    Name = "custom:referral_source",
                    Value = request.ReferralSource
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
                    Value = request.MarketingPreference.ToString()
                },
                // The 'address' attribute seems to be a required, not custom attribute, and should not have the 'custom:' prefix
                new AttributeType
                {
                    Name = "address",
                    Value = request.Place.Address
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
            Id = Guid.Parse(response.UserSub),
            Email = request.Email,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            Place = new PlaceDbObject()
            {
                PlaceId = request.Place.PlaceId,
                Address = request.Place.Address,
                Location = new LocationDbObject()
                {
                    Lat = request.Place.Location.Lat,
                    Lng = request.Place.Location.Lng
                },
                Viewport = new ViewPortDbObject()
                {
                    Northeast = new LocationDbObject()
                    {
                        Lat = request.Place.Viewport.Northeast.Lat,
                        Lng = request.Place.Viewport.Northeast.Lng
                    },
                    Southwest = new LocationDbObject()
                    {
                        Lat = request.Place.Viewport.Southwest.Lat,
                        Lng = request.Place.Viewport.Southwest.Lng
                    }
                }
            },
            CompanyName = request.CompanyName,
            CompanyType = request.CompanyType,
            CompanySize = request.CompanySize,
            ReferralSource = request.ReferralSource,
            CompanyPriority = request.CompanyPriority,
            MarketingPreference = request.MarketingPreference,
            CreatedDate = DateTime.UtcNow,
        };

        await _userRepository.AddUserAsync(user, ctx);

        return response;
    }

    public async Task<AuthenticatedUserResponse?> LoginAsync(LoginRequest request, CancellationToken ctx)
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

        var authResponse = await _cognitoService.InitiateAuthAsync(authRequest, ctx);

        if (authResponse.HttpStatusCode != System.Net.HttpStatusCode.OK) return null;

        var idToken = authResponse.AuthenticationResult?.IdToken;

        if (string.IsNullOrEmpty(idToken)) return null;

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(idToken) as JwtSecurityToken;
        var userIdString = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId)) return null;

        var user = await _userRepository.GetCustomerById(userId, ctx);

        if (user == null) return null;

        return new AuthenticatedUserResponse
        {
            AuthResponse = authResponse,
            User = user
        };
    }

    public DateTime? GetExpiryDateFromJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
        {
            return null;
        }

        try
        {
            var jwtToken = handler.ReadJwtToken(token);
            var expClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "exp");
            if (expClaim != null && long.TryParse(expClaim.Value, out var exp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return null;
    }

    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(string email, CancellationToken ctx)
    {
        var request = new ForgotPasswordRequest
        {
            ClientId = _appSettings.AppClientId,
            Username = email
        };


        var response = await _cognitoService.ForgotPasswordAsync(request, ctx);
        return response;
    }
    
    public async Task<ConfirmForgotPasswordResponse> ChangePasswordAsync(ChangedForgottenPasswordRequest request, CancellationToken ctx)
    {
        var awsRequest = new ConfirmForgotPasswordRequest
        {
            ClientId = _appSettings.AppClientId,
            Username = request.Email,
            Password = request.NewPassword,
            ConfirmationCode = request.ResetConfirmationCode
        };

        var response = await _cognitoService.ConfirmForgotPasswordAsync(awsRequest, ctx);
        return response;
    }
}