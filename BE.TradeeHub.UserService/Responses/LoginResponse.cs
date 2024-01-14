using BE.TradeeHub.UserService.Infrastructure.DbObjects;

namespace BE.TradeeHub.UserService.Responses;

public class LoginResponse
{
    public bool IsSuccess { get; set; }
    public bool IsConfirmed { get; set; }
    public UserDbObject? User { get; set; }
}