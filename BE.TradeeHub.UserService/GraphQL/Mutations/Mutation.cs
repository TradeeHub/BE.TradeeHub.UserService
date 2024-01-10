using BE.TradeeHub.UserService.Requests;
using BE.TradeeHub.UserService.Responses;
using HotChocolate.Execution;

namespace BE.TradeeHub.UserService.GraphQL.Mutations;

public class Mutation
{
    public async Task<TokenResponse> LoginAsync(LoginRequest request, [Service] AuthService authService)
    {
        try
        {
            var response = await authService.LoginAsync(request);
            // You won't be able to set cookies directly as in a REST API.
            // The client will need to handle the received token appropriately.
            return response;
        }
        catch (Exception ex)
        {
            throw new QueryException(ex.Message);
        }
    }
}