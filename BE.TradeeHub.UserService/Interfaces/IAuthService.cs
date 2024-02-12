using Amazon.CognitoIdentityProvider.Model;
using BE.TradeeHub.UserService.Domain.Entities;
using BE.TradeeHub.UserService.Requests;
using BE.TradeeHub.UserService.Responses;

namespace BE.TradeeHub.UserService.Interfaces;

public interface IAuthService
{
    Task<ConfirmSignUpResponse> ConfirmRegistrationAsync(string confirmationCode, string email, CancellationToken ctx);
    Task<ResendConfirmationCodeResponse> ResendVerificationCodeAsync(string email, CancellationToken ctx);
    Task<UserEntity> AddRandomUser(RegisterRequest request, CancellationToken ctx);
    Task<SignUpResponse> RegisterAsync(RegisterRequest request, CancellationToken ctx);
    Task<AuthenticatedUserResponse?> LoginAsync(LoginRequest request, CancellationToken ctx);
    Task<ForgotPasswordResponse> ForgotPasswordAsync(string email, CancellationToken ctx);
    Task<ConfirmForgotPasswordResponse> ChangePasswordAsync(ChangedForgottenPasswordRequest request, CancellationToken ctx);
    Task<RefreshJwtResponse> RefreshTokenAsync(IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken);
    Task<string> RefreshTokenAsync(HttpContext httpContext, string refreshToken, CancellationToken cancellationToken, string? deviceKey = null);
}