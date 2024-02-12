using BE.TradeeHub.UserService.Domain.Entities;

namespace BE.TradeeHub.UserService.Responses;

public class LoginResponse
{
    public bool IsSuccess { get; set; }
    public bool IsConfirmed { get; set; }
    public UserEntity? User { get; set; }
}