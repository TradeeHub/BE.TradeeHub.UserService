using BE.TradeeHub.UserService.Infrastructure.DbObjects;
using HotChocolate.Data;
using MongoDB.Driver;

namespace BE.TradeeHub.UserService.GraphQL.Queries;

public class Query
{
    // [UsePaging(MaxPageSize = 1000)]
    // [UseProjection]
    // [UseSorting]
    // [UseFiltering]
    public IExecutable<UserDbObject> GetUsers([Service] IMongoCollection<UserDbObject> collection, CancellationToken ctx)
    {
        var collect = collection.AsExecutable();
        return collect;
    }
}