using BE.TradeeHub.UserService.GraphQL.DataLoaders;
using BE.TradeeHub.UserService.Infrastructure.DbObjects;

namespace BE.TradeeHub.UserService.GraphQL.QueryResolvers;

public class TypeResolver
{
    public async Task<IEnumerable<UserDbObject>?> GetStaffMembers(
        [Parent] UserDbObject user, [Service] StaffDataLoader staffDataLoader, 
        CancellationToken ctx)
    {
        if (user.Staff != null)
        {
            return await staffDataLoader.LoadAsync(user.Id, ctx);
        }
        
        return null;
    }
    
    public async Task<IEnumerable<UserDbObject>?> GetCompaniesImMemberOf(
        [Parent] UserDbObject user, [Service] CompaniesMemberOfDataLoader companiesMemberOfDataLoader, 
        CancellationToken ctx)
    {
        if (user.CompaniesMemberOf != null)
        {
            return await companiesMemberOfDataLoader.LoadAsync(user.Id, ctx);
        }
        
        return null;
    }
}