using System.IdentityModel.Tokens.Jwt;
using BE.TradeeHub.UserService.Infrastructure.DbObjects;
using BE.TradeeHub.UserService.Requests;
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
}
