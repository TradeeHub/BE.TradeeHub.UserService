using Amazon.CognitoIdentityProvider.Model;

namespace BE.TradeeHub.UserService.Responses;

public class RefreshJwtResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public InitiateAuthResponse? AuthResponse { get; set; }
}