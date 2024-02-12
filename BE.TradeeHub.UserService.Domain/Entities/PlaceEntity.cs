namespace BE.TradeeHub.UserService.Domain.Entities;

public class PlaceEntity
{
    public string PlaceId { get; set; }
    public string Address { get; set; }
    public string Country { get; set; }
    public string CountryCode { get; set; }
    public string CallingCode { get; set; }
    public LocationEntity Location { get; set; }
    public ViewportEntity Viewport { get; set; }
}