namespace API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs
{
    public class DeliveryRouteResponseDto
    {
        public Guid DeliveryPlanId { get; set; }
        public List<DeliveryStopDto> Stops { get; set; } = new();
        public List<RouteGroupDto> RouteGroups { get; set; } = new();
    }
}
