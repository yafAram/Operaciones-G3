namespace API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs
{
    public class DeliveryAssignmentOverviewDto
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public Guid? ChainId { get; set; }
        public string? ChainName { get; set; }
        public Guid? TargetId { get; set; }
        public Guid? PlanId { get; set; }


        // Datos GPS y Contacto (Preservados para la UI)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Phone { get; set; }
        public string? BasePhone { get; set; }
        public string? Extension { get; set; }

        public Guid? AssignedDeliveryUserId { get; set; }
        public string? AssignedDeliveryUserName { get; set; }

        public int OrdersCount { get; set; }
        public decimal TotalAmount { get; set; }

        public bool HasAssignedDriver => AssignedDeliveryUserId.HasValue;

        public List<OrderSimpleDto> Orders { get; set; } = new();
    }

    public class OrderSimpleDto
    {
        public Guid OrderId { get; set; }
        public Guid? DeliveryStopId { get; set; }

        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public string OrderStatus { get; set; } = string.Empty;
        public string OfficialDeliveryStatus { get; set; } = string.Empty;

        public string? DeliveryPlanStatus { get; set; }
        public string? FailureReason { get; set; }

        public bool HasIncident { get; set; }
        public bool IsDelivered { get; set; }

        public List<OrderItemSimpleDto> Items { get; set; } = new();
    }

    public class OrderItemSimpleDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }
}