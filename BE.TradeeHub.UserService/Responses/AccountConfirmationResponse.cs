using Amazon.CognitoIdentityProvider.Model;

namespace BE.TradeeHub.UserService.Responses;

public class AccountConfirmationResponse
{
    public ConfirmSignUpResponse? ConfirmSignUpResponse { get; set; }
    public bool IsConfirmationSuccess { get; set; }
    public string? Message { get; set; }
}