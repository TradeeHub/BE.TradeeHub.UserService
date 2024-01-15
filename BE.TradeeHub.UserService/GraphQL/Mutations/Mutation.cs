using Amazon.CognitoIdentityProvider.Model;
using BE.TradeeHub.UserService.Requests;
using BE.TradeeHub.UserService.Responses;
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

    public async Task<LoginResponse> LoginAsync(LoginRequest request, [Service] AuthService authService,
        [Service] IHttpContextAccessor httpContextAccessor, CancellationToken ctx)
    {
        try
        {
            var authResponse = await authService.LoginAsync(request, ctx);

            if (authResponse == null || httpContextAccessor.HttpContext == null) return new LoginResponse();

            var idToken = authResponse.AuthResponse.AuthenticationResult?.IdToken;
            var refreshToken = authResponse.AuthResponse.AuthenticationResult?.RefreshToken;
            var expiresIn =
                authResponse.AuthResponse.AuthenticationResult?.ExpiresIn ?? 3600; // Default to 1 hour if null

            // Calculate the expiration time for the cookies
            var expirationTime = DateTime.UtcNow.AddSeconds(expiresIn);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Remember to change to true when using HTTPS
                SameSite = SameSiteMode.Unspecified, // Change to None when using Secure=true
                Expires = expirationTime,
                // Domain = "localhost", // Set the domain to localhost
                Path = "/" // Make the cookie available across the entire domain
            };
            // Set the IdToken as a cookie
            httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", idToken, cookieOptions);

            var refreshTokenExpiry = authService.GetExpiryDateFromJwt(refreshToken);
            
            var refreshTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Remember to change to true when using HTTPS
                SameSite = SameSiteMode.Unspecified, // Change to None when using Secure=true
                Expires = DateTime.UtcNow.AddDays(30),
                // Domain = "localhost", // Set the domain to localhost
                Path = "/" // Make the cookie available across the entire domain
            };
       
            httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, refreshTokenCookieOptions);

            return new LoginResponse()
            {
                User = authResponse.User,
                IsSuccess = true,
                IsConfirmed = true
            };
        }
        catch (UserNotConfirmedException)
        {
            return new LoginResponse()
            {
                IsSuccess = true,
                IsConfirmed = false
            };
        }
        catch (NotAuthorizedException ex)
        {
            // This block will catch the NotAuthorizedException, which indicates wrong username or password
            return new LoginResponse()
            {
                IsSuccess = false,
                IsConfirmed = false,
            };
        }
        catch (UserNotFoundException ex)
        {
            // This block will catch exception when the user does not exist
            return new LoginResponse()
            {
                IsSuccess = false,
                IsConfirmed = false,
            };
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            throw new QueryException(ex.Message);
        }
    }

    public async Task<AccountConfirmationResponse> ConfirmAccountAsync([Service] AuthService authService,
        string confirmationCode, string email, CancellationToken ctx)
    {
        try
        {
            var response = await authService.ConfirmRegistrationAsync(confirmationCode, email, ctx);

            return new AccountConfirmationResponse()
            {
                ConfirmSignUpResponse = response,
                IsConfirmationSuccess = true,
                Message = "Account successfully confirmed."
            };
        }
        catch (ExpiredCodeException ex)
        {
            return new AccountConfirmationResponse()
            {
                IsConfirmationSuccess = false,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            throw new QueryException(ex.Message);
        }
    }

    public async Task<ResendConfirmationCodeResponse> ResendVerificationCodeAsync([Service] AuthService authService,
        string email, CancellationToken ctx)
    {
        try
        {
            return await authService.ResendVerificationCodeAsync(email, ctx);
        }
        catch (Exception ex)
        {
            throw new QueryException(ex.Message);
        }
    }
}