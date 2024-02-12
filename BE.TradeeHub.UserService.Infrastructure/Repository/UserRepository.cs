﻿using BE.TradeeHub.UserService.Domain.Entities;
using BE.TradeeHub.UserService.Domain.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE.TradeeHub.UserService.Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly MongoDbContext _dbContext;

    public UserRepository(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<UserEntity?> GetCustomerById(Guid awsUserId, CancellationToken ctx)
    {
        var filter = Builders<UserEntity>.Filter.Eq(user => user.Id, awsUserId);

        return await _dbContext.Users.Find(filter).FirstOrDefaultAsync(ctx);
    }
    
    public async Task<IEnumerable<UserEntity>?> GetStaffByIds(IEnumerable<Guid> staffIds, CancellationToken ctx)
    {
        // The filter should be on the Customers field, not the Id field
        var filter = Builders<UserEntity>.Filter.AnyIn(p => p.Staff, staffIds);
        var cursor = await _dbContext.Users.FindAsync(filter, cancellationToken: ctx);
        var staff = await cursor.ToListAsync(ctx);

        return staff; 
    }
    
    public async Task<IEnumerable<UserEntity>?> GetCompaniesMemberOfByIds(IEnumerable<Guid> companyIds, CancellationToken ctx)
    {
        // The filter should be on the Customers field, not the Id field
        var filter = Builders<UserEntity>.Filter.AnyIn(p => p.CompaniesMemberOf, companyIds);
        var cursor = await _dbContext.Users.FindAsync(filter, cancellationToken: ctx);
        var companies = await cursor.ToListAsync(ctx);

        return companies; 
    }
    
    public async Task AddUserAsync(UserEntity user, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Users.InsertOneAsync(user, null, cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception($"Error adding user: {e.Message}");
        }
    }

    public async Task UpdateUserEmailVerifiedStatus(string email, bool isVerified, CancellationToken cancellationToken)
    {
        var filter = Builders<UserEntity>.Filter.Eq(u => u.Email, email);
        var update = Builders<UserEntity>.Update
            .Set(u => u.EmailVerified, isVerified)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);
        
        await _dbContext.Users.UpdateOneAsync(filter, update, null, cancellationToken);
    }
}