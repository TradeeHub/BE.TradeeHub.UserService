namespace BE.TradeeHub.UserService.Requests;

public class PlaceRequest
{
    public string PlaceId { get; set; }
    public string Address { get; set; }
    public LocationRequest  Location { get; set; }
    public ViewportRequest Viewport { get; set; }
}