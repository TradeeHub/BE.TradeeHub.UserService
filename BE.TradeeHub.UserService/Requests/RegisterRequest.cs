namespace BE.TradeeHub.UserService.Requests;

public class RegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public PlaceRequest Place { get; set; }
    public string CompanyName { get; set; }
    public string CompanyType { get; set; }
    public string CompanySize { get; set; }
    public string ReferralSource { get; set; }
    public string CompanyPriority { get; set; }
    public bool MarketingPreference { get; set; }
}

