using BE.TradeeHub.UserService.Domain.Entities;
using BE.TradeeHub.UserService.Domain.Interfaces.Repositories;
using BE.TradeeHub.UserService.Interfaces;

namespace BE.TradeeHub.UserService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<UserEntity?> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUserByIdAsync(id, cancellationToken);
    }
}