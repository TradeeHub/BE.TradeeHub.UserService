using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BE.TradeeHub.UserService.Infrastructure.DbObjects;

public class UserDbObject 
{
    [BsonId]
    public Guid Id { get; set; } //Aws Cognito generated UUID
    public string Email { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public PlaceDbObject Place { get; set; }
    public string CompanyName { get; set; }
    public string CompanyType { get; set; }
    public string CompanySize { get; set; }
    public string ReferralSource { get; set; }
    public string CompanyPriority { get; set; }
    public bool MarketingPreference { get; set; }
    public IEnumerable<Guid>? Staff { get; set; }
    public IEnumerable<Guid>? CompaniesMemberOf { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
}