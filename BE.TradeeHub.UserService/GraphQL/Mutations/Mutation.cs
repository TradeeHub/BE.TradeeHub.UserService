using Amazon.CognitoIdentityProvider.Model;
using BE.TradeeHub.UserService.Infrastructure.DbObjects;
using BE.TradeeHub.UserService.Requests;
using HotChocolate.Execution;

namespace BE.TradeeHub.UserService.GraphQL.Mutations;

public class Mutation
{
    public async Task<SignUpResponse> RegisterAsync([Service] AuthService authService, RegisterRequest request,
        CancellationToken ctx)
    {
        try
        {
            var response = await authService.RegisterAsync(request, ctx);
            
            return response;
        }
        catch (Exception ex)
        {
            throw new QueryException(ex.Message);
        }
    }
    
    public async Task<UserDbObject> AddRandomUser([Service] AuthService authService, RegisterRequest request,
        CancellationToken ctx)
    {
        try
        {
            var response = await authService.AddRandomUser(request, ctx);
            
            return response;
        }
        catch (Exception ex)
        {
            throw new QueryException(ex.Message);
        }
    }

    public async Task<ConfirmSignUpResponse> ConfirmRegistrationAsync([Service] AuthService authService,
        string confirmationCode, string email, CancellationToken ctx)
    {
        try
        {
            var response = await authService.ConfirmRegistrationAsync(confirmationCode, email, ctx);

            return response;
        }
        catch (Exception ex)
        {
            throw new QueryException(ex.Message);
        }
    }

    public async Task<ResendConfirmationCodeResponse> ResendConfirmationCodeAsync([Service] AuthService authService,
        string email, CancellationToken ctx)
    {
        try
        {
            return await authService.ResendConfirmationCodeAsync(email, ctx);
        }
        catch (Exception ex)
        {
            throw new QueryException(ex.Message);
        }
    }
}