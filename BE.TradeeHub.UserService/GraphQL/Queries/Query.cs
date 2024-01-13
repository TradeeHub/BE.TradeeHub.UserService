using BE.TradeeHub.UserService.Infrastructure.DbObjects;
using BE.TradeeHub.UserService.Requests;
using BE.TradeeHub.UserService.Responses;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Execution;
using MongoDB.Driver;

namespace BE.TradeeHub.UserService.GraphQL.Queries;

public class Query
{
    // [UsePaging(MaxPageSize = 1000)]
    // [UseProjection]
    // [UseSorting]
    // [UseFiltering]
    // public IExecutable<UserDbObject> GetUsers([Service] IMongoCollection<UserDbObject> collection, CancellationToken ctx)
    // {
    //     var collect = collection.AsExecutable();
    //     return collect;
    // }
    
    public async Task<TokenResponse> LoginAsync(LoginRequest request, [Service] AuthService authService, [Service] IHttpContextAccessor httpContextAccessor)
    {
        try
        {
            var response = await authService.LoginAsync(request);
            
            if (httpContextAccessor.HttpContext != null)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Set to false if not using HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1) // Set the expiry as needed
                };

                // Assuming you want to set the IdToken as a cookie
                httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", response.IdToken, cookieOptions);
            }
            
            return response;
        }
        catch (Exception ex)
        {
            throw new QueryException(ex.Message);
        }
    }
    
    // [Authorize]
    // public async Task<UserDbObject> TestAuth()
    // {
    //     return new UserDbObject() { Name = "Glen" };
    // }
}
