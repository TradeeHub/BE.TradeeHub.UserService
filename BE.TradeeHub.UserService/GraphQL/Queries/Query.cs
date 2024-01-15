using System.IdentityModel.Tokens.Jwt;
using BE.TradeeHub.UserService.Infrastructure.DbObjects;
using BE.TradeeHub.UserService.Requests;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Execution;
using MongoDB.Driver;

namespace BE.TradeeHub.UserService.GraphQL.Queries;

public class Query
{
    [UsePaging(MaxPageSize = 100)]
    [UseProjection]
    [HotChocolate.Types.UseSorting]
    [HotChocolate.Types.UseFiltering]
    public IExecutable<UserDbObject> GetUsers([Service] IMongoCollection<UserDbObject> collection, CancellationToken ctx)
    {
        var collect = collection.AsExecutable();
        return collect;
    }
    
    [Authorize]
    public IExecutable<UserDbObject> GetUserByAwsCognitoId([Service] IMongoCollection<UserDbObject> collection, Guid Id, CancellationToken ctx)
    {
        return collection.Find(x => x.AwsCognitoUserId == Id).AsExecutable();
    }
}
