using BE.TradeeHub.UserService.Domain.Entities;

namespace BE.TradeeHub.UserService.Interfaces;

public interface IUserService
{
    Task<UserEntity?> GetUserAsync(Guid id, CancellationToken cancellationToken);
}