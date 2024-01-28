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
            var deviceKey = authResponse.AuthResponse.AuthenticationResult?.NewDeviceMetadata?.DeviceKey;

            var expiresIn = authResponse.AuthResponse.AuthenticationResult?.ExpiresIn ?? 3600; // Default to 1 hour if null

            // Calculate the expiration time for the cookies
            var expirationTime = DateTime.UtcNow.AddSeconds(expiresIn);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Remember to change to true when using HTTPS
                SameSite = SameSiteMode.Strict, // Change to None when using Secure=true
                Expires = expirationTime,
                Path = "/" // Make the cookie available across the entire domain
            };
            // Set the IdToken as a cookie
            httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", idToken, cookieOptions);
            
            var refreshTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Remember to change to true when using HTTPS
                SameSite = SameSiteMode.Strict, // Change to None when using Secure=true
                Expires = DateTime.UtcNow.AddYears(5),
                Path = "/" // Make the cookie available across the entire domain
            };

            httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, refreshTokenCookieOptions);
            
            if (request.RememberMe && !string.IsNullOrEmpty(deviceKey))
            {
                httpContextAccessor.HttpContext.Response.Cookies.Append("deviceKey", deviceKey, refreshTokenCookieOptions);
            }

            return new LoginResponse()
            {
                User = authResponse.User,
                IsSuccess = true,
                IsConfirmed = true
            };
        }
        catch (UserNotConfirmedException ex)
        {
            return new LoginResponse()
            {
                User = null,
                IsSuccess = true,
                IsConfirmed = false
            };
        }
        catch (NotAuthorizedException ex)
        {
            // This block will catch the NotAuthorizedException, which indicates wrong username or password
            throw new QueryException(ex.Message);
        }
        catch (UserNotFoundException ex)
        {
            // This block will catch exception when the user does not exist
            throw new QueryException(ex.Message);

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

            return response.HttpStatusCode switch
            {
                System.Net.HttpStatusCode.OK => new AccountConfirmationResponse()
                {
                    ConfirmSignUpResponse = response,
                    IsConfirmationSuccess = true,
                    Message = "Account successfully confirmed."
                },
                _ => new AccountConfirmationResponse()
                {
                    ConfirmSignUpResponse = response,
                    IsConfirmationSuccess = false,
                    Message = "Invalid verification code provided, please try again."
                }
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

    public LogoutResponse Logout([Service] IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext == null)
        {
            return new LogoutResponse() { Success = false, Message = "Failed to access HTTP context." };
        }

        // Overwrite and expire the JWT cookie
        httpContextAccessor.HttpContext.Response.Cookies.Append("jwt", "", new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(-1),
            HttpOnly = true,
            Secure = false, // Remember to change to true when using HTTPS
            SameSite = SameSiteMode.Strict, // Change to None when using Secure=true
            Path = "/"
        });

        // Overwrite and expire the refreshToken cookie
        httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", "", new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(-1),
            HttpOnly = true,
            Secure = false, // Remember to change to true when using HTTPS
            SameSite = SameSiteMode.Strict, // Change to None when using Secure=true
            Path = "/"
        });
        
        httpContextAccessor.HttpContext.Response.Cookies.Append("deviceKey", "", new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(-1),
            HttpOnly = true,
            Secure = false, // Remember to change to true when using HTTPS
            SameSite = SameSiteMode.Strict, // Change to None when using Secure=true
            Path = "/"
        });
        
        return new LogoutResponse() { Success = true, Message = "Logout Successful." };
    }
    
    public async Task<ForgotPasswordResponse> ForgotPasswordAsync([Service] AuthService authService, string email, CancellationToken ctx)
    {
        try
        {
            return await authService.ForgotPasswordAsync(email, ctx);
        }
        catch (Exception ex)
        {
            throw new QueryException(ex.Message);
        }
    }
    
    public async Task<ConfirmForgotPasswordResponse> ChangePassword([Service] AuthService authService, ChangedForgottenPasswordRequest request, CancellationToken ctx)
    {
        try
        {
            return await authService.ChangePasswordAsync(request, ctx);
        }
        catch (Exception ex)
        {
            throw new QueryException(ex.Message);
        }
    }
}