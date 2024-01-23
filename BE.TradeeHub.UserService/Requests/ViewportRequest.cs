namespace BE.TradeeHub.UserService.Requests;

public class ViewportRequest
{
    public LocationRequest Northeast { get; set; }
    public LocationRequest Southwest { get; set; }
}