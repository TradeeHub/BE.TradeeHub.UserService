using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using BE.TradeeHub.UserService.Mutations;

namespace BE.TradeeHub.UserService;

public class AuthService
{
    private readonly IAmazonCognitoIdentityProvider _cognitoService;
    
    public AuthService(IAmazonCognitoIdentityProvider cognitoService)
    {
        _cognitoService = cognitoService;
    }
    
    public async Task SignUpUserAsync(RegisterRequest request)
    {
        var signUpRequest = new SignUpRequest
        {
            ClientId = "61u3jgn4afgsj8e7v7209fn5l",
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

        var response = await _cognitoService.SignUpAsync(signUpRequest);
    }
    
    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var authRequest = new InitiateAuthRequest
        {
            ClientId = "61u3jgn4afgsj8e7v7209fn5l", // Replace with your actual client id
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