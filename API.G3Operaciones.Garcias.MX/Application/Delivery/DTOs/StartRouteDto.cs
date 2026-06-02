namespace API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs
{
    public class StartRouteDto
    {
        public Guid DeliveryUserId { get; set; }
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
    }
}
