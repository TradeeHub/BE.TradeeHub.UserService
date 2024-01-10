namespace BE.TradeeHub.UserService.Responses;

public class TokenResponse
{
    public string IdToken { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}