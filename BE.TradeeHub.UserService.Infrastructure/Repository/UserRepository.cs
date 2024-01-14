﻿using BE.TradeeHub.UserService.Infrastructure.DbObjects;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE.TradeeHub.UserService.Infrastructure.Repository;

public class UserRepository
{
    private readonly MongoDbContext _dbContext;

    public UserRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<UserDbObject> GetCustomerById(ObjectId userId)
    {
        var filter = Builders<UserDbObject>.Filter.Eq(c => c.Id, userId);
        var user =  await _dbContext.Users.Find(filter).FirstOrDefaultAsync();
        return user;
    }
    
    public async Task<UserDbObject?> GetCustomerByAwsId(Guid awsUserId, CancellationToken ctx)
    {
        var filter = Builders<UserDbObject>.Filter.Eq(user => user.AwsCognitoUserId, awsUserId);

        return await _dbContext.Users.Find(filter).FirstOrDefaultAsync(ctx);
    }

    
    public async Task<IEnumerable<UserDbObject>?> GetStaffByIds(IEnumerable<ObjectId> staffIds, CancellationToken ctx)
    {
        // The filter should be on the Customers field, not the Id field
        var filter = Builders<UserDbObject>.Filter.AnyIn(p => p.Staff, staffIds);
        var cursor = await _dbContext.Users.FindAsync(filter, cancellationToken: ctx);
        var staff = await cursor.ToListAsync(ctx);

        return staff; 
    }
    
    public async Task<IEnumerable<UserDbObject>?> GetCompaniesMemberOfByIds(IEnumerable<ObjectId> companyIds, CancellationToken ctx)
    {
        // The filter should be on the Customers field, not the Id field
        var filter = Builders<UserDbObject>.Filter.AnyIn(p => p.CompaniesMemberOf, companyIds);
        var cursor = await _dbContext.Users.FindAsync(filter, cancellationToken: ctx);
        var companies = await cursor.ToListAsync(ctx);

        return companies; 
    }
    
    public async Task AddUserAsync(UserDbObject user, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Users.InsertOneAsync(user, null, cancellationToken);
        }
        catch (Exception e)
        {
            var error = e.Message;
        }
    }

    public async Task UpdateUserEmailVerifiedStatus(string email, bool isVerified, CancellationToken cancellationToken)
    {
        var filter = Builders<UserDbObject>.Filter.Eq(u => u.Email, email);
        var update = Builders<UserDbObject>.Update
            .Set(u => u.EmailVerified, isVerified)
            .Set(u => u.UpdatedDate, DateTime.UtcNow);
        
        await _dbContext.Users.UpdateOneAsync(filter, update, null, cancellationToken);
    }
}