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

    public string Name => _httpContextAccessor.HttpContext?.User.FindFirst("name")?.Value ?? throw new Exception("Name claim not found.");
    public string Email => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? throw new Exception("Email claim not found.");
    public string CompanyName => _httpContextAccessor.HttpContext?.User.FindFirst("custom:company_name")?.Value ?? throw new Exception("Company Name claim not found.");
    public string LocationLat => _httpContextAccessor.HttpContext?.User.FindFirst("custom:location_lat")?.Value ?? throw new Exception("Location Lat claim not found.");
    public string LocationLng=> _httpContextAccessor.HttpContext?.User.FindFirst("custom:location_lng")?.Value ?? throw new Exception("Location Lng claim not found.");
    public string Country => _httpContextAccessor.HttpContext?.User.FindFirst("custom:country")?.Value ?? throw new Exception("Country claim not found.");
    public string CountryCode => _httpContextAccessor.HttpContext?.User.FindFirst("custom:country_code")?.Value ?? throw new Exception("Country Code claim not found.");
    public string CallingCode => _httpContextAccessor.HttpContext?.User.FindFirst("custom:calling_code")?.Value ?? throw new Exception("Calling Code claim not found.");
}