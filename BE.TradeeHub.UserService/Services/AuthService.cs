using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using BE.TradeeHub.UserService.Domain.Entities;
using BE.TradeeHub.UserService.Domain.Interfaces;
using BE.TradeeHub.UserService.Domain.Interfaces.Repositories;
using BE.TradeeHub.UserService.Interfaces;
using BE.TradeeHub.UserService.Requests;
using BE.TradeeHub.UserService.Responses;

namespace BE.TradeeHub.UserService.Services;

public class AuthService : IAuthService
{
    private readonly IAmazonCognitoIdentityProvider _cognitoService;
    private readonly IAppSettings _appSettings;
    private readonly IUserRepository _userRepository;

    public AuthService(IAmazonCognitoIdentityProvider cognitoService, IAppSettings appSettings,
        IUserRepository userRepository)
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
            await _userRepository.UpdateUserEmailVerifiedStatusAsync(email, true, ctx);
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

    public async Task<UserEntity> AddRandomUser(RegisterRequest request, CancellationToken ctx)
    {
        var user = new UserEntity
        {
            Email = request.Email,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            Place = new PlaceEntity()
            {
                PlaceId = request.Place.PlaceId,
                Address = request.Place.Address,
                Location = new LocationEntity()
                {
                    Lat = request.Place.Location.Lat,
                    Lng = request.Place.Location.Lng
                },
                Viewport = new ViewportEntity()
                {
                    Northeast = new LocationEntity()
                    {
                        Lat = request.Place.Viewport.Northeast.Lat,
                        Lng = request.Place.Viewport.Northeast.Lng
                    },
                    Southwest = new LocationEntity()
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
            CreatedAt = DateTime.UtcNow,
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
                    Name = "custom:location_lat",
                    Value = request.Place.Location.Lat.ToString(CultureInfo.InvariantCulture)
                },
                new AttributeType
                {
                    Name = "custom:location_lng",
                    Value = request.Place.Location.Lng.ToString(CultureInfo.InvariantCulture)
                },
                new AttributeType
                {
                    Name = "custom:place_id",
                    Value = request.Place.PlaceId.ToString(CultureInfo.InvariantCulture)
                },
                new AttributeType
                {
                    Name = "custom:calling_code",
                    Value = request.Place.CallingCode.ToString(CultureInfo.InvariantCulture)
                },
                new AttributeType
                {
                    Name = "custom:country",
                    Value = request.Place.Country.ToString(CultureInfo.InvariantCulture)
                },
                new AttributeType
                {
                    Name = "custom:country_code",
                    Value = request.Place.CountryCode.ToString(CultureInfo.InvariantCulture)
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

        var user = new UserEntity
        {
            Id = Guid.Parse(response.UserSub),
            Email = request.Email,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            Place = new PlaceEntity()
            {
                PlaceId = request.Place.PlaceId,
                Address = request.Place.Address,
                Country = request.Place.Country,
                CountryCode = request.Place.CountryCode,
                CallingCode = request.Place.CallingCode,
                Location = new LocationEntity()
                {
                    Lat = request.Place.Location.Lat,
                    Lng = request.Place.Location.Lng
                },
                Viewport = new ViewportEntity()
                {
                    Northeast = new LocationEntity()
                    {
                        Lat = request.Place.Viewport.Northeast.Lat,
                        Lng = request.Place.Viewport.Northeast.Lng
                    },
                    Southwest = new LocationEntity()
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
            CreatedAt = DateTime.UtcNow,
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

        var user = await _userRepository.GetUserByIdAsync(userId, ctx);

        if (user == null) return null;

        return new AuthenticatedUserResponse
        {
            AuthResponse = authResponse,
            User = user
        };
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

    public async Task<ConfirmForgotPasswordResponse> ChangePasswordAsync(ChangedForgottenPasswordRequest request,
        CancellationToken ctx)
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

    public async Task<RefreshJwtResponse> RefreshTokenAsync(IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken)
    {
        try
        {
            // var jwtCookieValue = httpContextAccessor.HttpContext.Request.Cookies["jwt"];
            var refreshToken = httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
            var deviceKey = httpContextAccessor.HttpContext.Request.Cookies["deviceKey"];

            if (string.IsNullOrEmpty(refreshToken)) return new RefreshJwtResponse() { Success = false, Message = "No refresh token found." };
            
            var tokenRequest = new InitiateAuthRequest
            {
                ClientId = _appSettings.AppClientId,
                AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "REFRESH_TOKEN", refreshToken }
                }
            };

            if (!string.IsNullOrEmpty(deviceKey))
            {
                tokenRequest.AuthParameters.Add("DEVICE_KEY", deviceKey);
            }

            var authResponse = await _cognitoService.InitiateAuthAsync(tokenRequest, cancellationToken);
            
            if (authResponse.HttpStatusCode != System.Net.HttpStatusCode.OK) return new RefreshJwtResponse() { Success = false, Message = "Invalid Refresh Token" };;
            
            var newJwtToken = authResponse.AuthenticationResult.IdToken;
            var jwtExpiresIn = authResponse.AuthenticationResult.ExpiresIn;
            var jwtExpirationTime = DateTime.UtcNow.AddSeconds(jwtExpiresIn);

            var jwtCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Change to true when using HTTPS
                SameSite = SameSiteMode.Strict, // Change to None when using Secure=true
                Expires = jwtExpirationTime,
                Path = "/" // Make the cookie available across the entire domain
            };

            httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", newJwtToken, jwtCookieOptions);
            
            return new RefreshJwtResponse
            {
                Success = true,
                AuthResponse = authResponse,
                Message = "Successfully refreshed JWT token."
            };
        }
        catch (Exception ex)
        {
            // Log the exception details
            Console.WriteLine(ex.Message);
            throw; // Rethrow the exception to handle it outside this method
        }
    }

    public async Task<string> RefreshTokenAsync(HttpContext httpContext, string refreshToken,
        CancellationToken cancellationToken, string? deviceKey = null)
    {
        try
        {
            var jwt = "";
            var tokenRequest = new InitiateAuthRequest
            {
                ClientId = _appSettings.AppClientId,
                AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "REFRESH_TOKEN", refreshToken }
                }
            };

            if (!string.IsNullOrEmpty(deviceKey))
            {
                tokenRequest.AuthParameters.Add("DEVICE_KEY", deviceKey);
            }

            var authResponse = await _cognitoService.InitiateAuthAsync(tokenRequest, cancellationToken);

            if (authResponse.HttpStatusCode != System.Net.HttpStatusCode.OK ||
                authResponse.AuthenticationResult == null)
                throw new Exception("Failed to refresh token");

            // Check if response has already started
            if (!httpContext.Response.HasStarted)
            {
                var newJwtToken = authResponse.AuthenticationResult.IdToken;
                var jwtExpiresIn = authResponse.AuthenticationResult.ExpiresIn;
                var jwtExpirationTime = DateTime.UtcNow.AddSeconds(jwtExpiresIn);

                var jwtCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Change to true when using HTTPS
                    SameSite = SameSiteMode.Strict, // Change to None when using Secure=true
                    Expires = jwtExpirationTime,
                    Path = "/" // Make the cookie available across the entire domain
                };

                httpContext.Response.Cookies.Append("jwt", newJwtToken, jwtCookieOptions);
                
                jwt = newJwtToken;
            }
            else
            {
                // Log or handle the case where the response has already started
                Console.WriteLine("Response has already started, cannot set cookie.");
            }

            return jwt;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}