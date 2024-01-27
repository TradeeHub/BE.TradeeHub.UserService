namespace BE.TradeeHub.UserService.Responses;

public class ChangedForgottenPasswordRequest
{
    public string ResetConfirmationCode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}