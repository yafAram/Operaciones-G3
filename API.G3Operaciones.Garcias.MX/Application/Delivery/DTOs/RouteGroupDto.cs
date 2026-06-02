namespace API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs
{
    public class RouteGroupDto
    {
        public int RouteGroup { get; set; }
        public string GoogleMapsUrl { get; set; } = "";
        public List<DeliveryStopDto> Stops { get; set; } = new();
    }
}
