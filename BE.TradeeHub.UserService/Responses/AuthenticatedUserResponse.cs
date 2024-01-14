using Amazon.CognitoIdentityProvider.Model;
using BE.TradeeHub.UserService.Infrastructure.DbObjects;

namespace BE.TradeeHub.UserService.Responses;

public class AuthenticatedUserResponse
{
    public UserDbObject User { get; set; }
    public InitiateAuthResponse AuthResponse { get; set; }
}