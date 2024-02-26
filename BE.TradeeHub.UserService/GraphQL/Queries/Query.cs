using BE.TradeeHub.UserService.Domain.Entities;
using HotChocolate.Authorization;
using HotChocolate.Data;
using MongoDB.Driver;

namespace BE.TradeeHub.UserService.GraphQL.Queries;

public class Query
{
    [UsePaging(MaxPageSize = 100)]
    [UseProjection]
    [HotChocolate.Types.UseSorting]
    [HotChocolate.Types.UseFiltering]
    public IExecutable<UserEntity> GetUsers([Service] IMongoCollection<UserEntity> collection, CancellationToken ctx)
    {
        var collect = collection.AsExecutable();
        return collect;
    }

    [Authorize]
    public IExecutable<UserEntity> GetUserByAwsCognitoId([Service] IMongoCollection<UserEntity> collection, Guid id,
        CancellationToken ctx)
    {
        return collection.Find(x => x.Id == id).AsExecutable();
    }

    [Authorize]
    [NodeResolver]
    public async Task<UserEntity?> GetUser([Service] IMongoCollection<UserEntity> collection, Guid id, CancellationToken ctx)
    {
        var filter = Builders<UserEntity>.Filter.Eq(x => x.Id, id);
        return await collection.Find(filter).FirstOrDefaultAsync(ctx);
    }

    [Authorize]
    [UseFirstOrDefault]
    public UserEntity GetLoggedInUser([Service] UserContext userContext)
    {
        return new UserEntity()
        {
            Id = userContext.UserId,
            Name = userContext.Name,
            CompanyName = userContext.CompanyName,
            Email = userContext.Email,
            Place = new PlaceEntity()
            {
                CallingCode = userContext.CallingCode,
                Country = userContext.Country,
                CountryCode = userContext.CountryCode,
                Location = new LocationEntity()
                {
                    Lat = Convert.ToDecimal(userContext.LocationLat),
                    Lng = Convert.ToDecimal(userContext.LocationLng)
                }
            }
        };
    }
}