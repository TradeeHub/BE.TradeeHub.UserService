namespace BE.TradeeHub.UserService.Infrastructure.DbObjects;

public class PlaceDbObject
{
    public string PlaceId { get; set; }
    public string Address { get; set; }
    public string Country { get; set; }
    public string CountryCode { get; set; }
    public string CallingCode { get; set; }
    public LocationDbObject Location { get; set; }
    public ViewPortDbObject Viewport { get; set; }
}