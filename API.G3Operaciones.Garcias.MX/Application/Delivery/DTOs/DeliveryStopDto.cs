using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;

namespace API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs
{
    public class DeliveryStopDto
    {
        public Guid DeliveryStopId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ClientId { get; set; }

        public string ClientName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public int Sequence { get; set; }
        public int RouteGroup { get; set; }

        public DeliveryStopStatus Status { get; set; }
        public Guid? TargetId { get; set; }
        public decimal OrderTotal { get; set; }
        public int ItemsCount { get; set; }
        public List<OrderItemSimpleDto> Items { get; set; }
    }
}
