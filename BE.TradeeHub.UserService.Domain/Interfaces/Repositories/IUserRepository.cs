using BE.TradeeHub.UserService.Domain.Entities;
using MongoDB.Bson;

namespace BE.TradeeHub.UserService.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetCustomerById(Guid awsUserId, CancellationToken ctx);

    Task<IEnumerable<UserEntity>?> GetStaffByIds(IEnumerable<Guid> staffIds, CancellationToken ctx);

    Task<IEnumerable<UserEntity>?> GetCompaniesMemberOfByIds(IEnumerable<Guid> companyIds, CancellationToken ctx);

    Task AddUserAsync(UserEntity user, CancellationToken cancellationToken);

    Task UpdateUserEmailVerifiedStatus(string email, bool isVerified, CancellationToken cancellationToken);
}