namespace API.G3Operaciones.Garcias.MX.Application.Orders.DTOs
{
    public class OrderChainGroupDto
    {
        public Guid? ChainId { get; set; }
        public string ChainName { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderBranchGroupDto> Branches { get; set; } = new();
    }

    public class OrderBranchGroupDto
    {
        public Guid? ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientAddress { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderSummaryDto> Orders { get; set; } = new();
    }

    public class OrderSummaryDto
    {
        public Guid OrderId { get; set; }
        public Guid OperatorId { get; set; }
        public decimal TotalAmount { get; set; }

        // Estado oficial de delivery: Entregado / Incidente / En ruta / Reprogramado / Pendiente
        public string Status { get; set; } = string.Empty;

        // Estado original de Order, para no perder trazabilidad
        public string OrderStatus { get; set; } = string.Empty;

        public Guid? DeliveryStopId { get; set; }
        public string? DeliveryPlanStatus { get; set; }
        public bool HasIncident { get; set; }
        public bool IsDelivered { get; set; }
        public string? FailureReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
