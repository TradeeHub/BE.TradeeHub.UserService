using Amazon.CognitoIdentityProvider.Model;
using BE.TradeeHub.UserService.Domain.Entities;

namespace BE.TradeeHub.UserService.Responses;

public class AuthenticatedUserResponse
{
    public UserEntity User { get; set; }
    public InitiateAuthResponse AuthResponse { get; set; }
}