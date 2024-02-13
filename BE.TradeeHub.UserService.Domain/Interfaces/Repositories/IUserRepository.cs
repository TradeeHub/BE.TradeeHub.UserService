using BE.TradeeHub.UserService.Domain.Entities;
using MongoDB.Bson;

namespace BE.TradeeHub.UserService.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetUserByIdAsync(Guid awsUserId, CancellationToken ctx);

    Task<List<UserEntity>?> GetUsersByIdsAsync(IEnumerable<Guid> awsUserIds, CancellationToken ctx);

    Task<IEnumerable<UserEntity>?> GetStaffByIdsAsync(IEnumerable<Guid> staffIds, CancellationToken ctx);

    Task<IEnumerable<UserEntity>?> GetCompaniesMemberOfByIdsAsync(IEnumerable<Guid> companyIds, CancellationToken ctx);

    Task AddUserAsync(UserEntity user, CancellationToken cancellationToken);

    Task UpdateUserEmailVerifiedStatusAsync(string email, bool isVerified, CancellationToken cancellationToken);
}