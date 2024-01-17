using System.Security.Claims;

namespace BE.TradeeHub.UserService;

public class UserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                throw new Exception("User ID claim not found or is not a valid GUID.");
            }
            return userId;
        }
    }

    public string Name => _httpContextAccessor.HttpContext?.User?.FindFirst("name")?.Value ?? throw new Exception("Name claim not found.");

    public string Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? throw new Exception("Email claim not found.");
}