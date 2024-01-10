using BE.TradeeHub.UserService.Domain.Enteties;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BE.TradeeHub.UserService.Domain.Entities;

public class User
{
    [BsonId]
    public ObjectId Id { get; set; }
    public Guid AwsCognitoUserId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Name { get; set; }
    public string CompanyName { get; set; }
    public string CompanyType { get; set; }
    public string Address { get; set; }
    public GeneralCompanyInfo GeneralInfo { get; set; }
    public IEnumerable<ObjectId>? Staff { get; set; }
    public IEnumerable<ObjectId>? CompaniesMemberOf { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
}