using BE.TradeeHub.UserService.Domain.Entities;
using BE.TradeeHub.UserService.GraphQL.DataLoaders;

namespace BE.TradeeHub.UserService.GraphQL.QueryResolvers;

public class TypeResolver
{
    public async Task<UserEntity?> GetUser(
        [Parent] UserEntity user, [Service] UserDataLoader userDataLoader, 
        CancellationToken ctx)
    {
        var userEntity = await userDataLoader.LoadAsync(user.Id, ctx);
        return userEntity.FirstOrDefault();   
    }
    
    public async Task<IEnumerable<UserEntity>?> GetStaffMembers(
        [Parent] UserEntity user, [Service] StaffDataLoader staffDataLoader, 
        CancellationToken ctx)
    {
        if (user.Staff != null)
        {
            return await staffDataLoader.LoadAsync(user.Id, ctx);
        }
        
        return null;
    }
    
    public async Task<IEnumerable<UserEntity>?> GetCompaniesImMemberOf(
        [Parent] UserEntity user, [Service] CompaniesMemberOfDataLoader companiesMemberOfDataLoader, 
        CancellationToken ctx)
    {
        if (user.CompaniesMemberOf != null)
        {
            return await companiesMemberOfDataLoader.LoadAsync(user.Id, ctx);
        }
        
        return null;
    }
}