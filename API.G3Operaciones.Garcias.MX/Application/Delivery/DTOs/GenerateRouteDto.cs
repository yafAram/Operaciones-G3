namespace API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs
{
    public class GenerateRouteDto
    {
        public Guid DeliveryUserId { get; set; }
        public DateTime Date { get; set; }

        // Ubicación actual del repartidor
        public decimal CurrentLatitude { get; set; }
        public decimal CurrentLongitude { get; set; }
    }
}
