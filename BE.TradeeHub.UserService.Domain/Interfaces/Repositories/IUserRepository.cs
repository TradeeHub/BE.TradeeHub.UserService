using BE.TradeeHub.UserService.Domain.Entities;
using MongoDB.Bson;

namespace BE.TradeeHub.UserService.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> GetCustomerById(ObjectId userId);

    Task<IEnumerable<User>?> GetStaffByIds(IEnumerable<ObjectId> staffIds, CancellationToken ctx);

    Task<IEnumerable<User>?> GetCompaniesMemberOfByIds(IEnumerable<ObjectId> companyIds, CancellationToken ctx);
}