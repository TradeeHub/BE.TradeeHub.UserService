using BE.TradeeHub.UserService.Domain.Interfaces.Repositories;
using BE.TradeeHub.UserService.Infrastructure.DbObjects;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE.TradeeHub.UserService.Infrastructure.Repository;

public class UserRepository(MongoDbContext dbContext)
{
    public async Task<UserDbObject> GetCustomerById(ObjectId userId)
    {
        var filter = Builders<UserDbObject>.Filter.Eq(c => c.Id, userId);
        var user =  await dbContext.Users.Find(filter).FirstOrDefaultAsync();
        return user;
    }
    
    public async Task<IEnumerable<UserDbObject>?> GetStaffByIds(IEnumerable<ObjectId> staffIds, CancellationToken ctx)
    {
        // The filter should be on the Customers field, not the Id field
        var filter = Builders<UserDbObject>.Filter.AnyIn(p => p.Staff, staffIds);
        var cursor = await dbContext.Users.FindAsync(filter, cancellationToken: ctx);
        var staff = await cursor.ToListAsync(ctx);

        return staff; 
    }
    
    public async Task<IEnumerable<UserDbObject>?> GetCompaniesMemberOfByIds(IEnumerable<ObjectId> companyIds, CancellationToken ctx)
    {
        // The filter should be on the Customers field, not the Id field
        var filter = Builders<UserDbObject>.Filter.AnyIn(p => p.CompaniesMemberOf, companyIds);
        var cursor = await dbContext.Users.FindAsync(filter, cancellationToken: ctx);
        var companies = await cursor.ToListAsync(ctx);

        return companies; 
    }
}