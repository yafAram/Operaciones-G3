using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;

namespace API.G3Operaciones.Garcias.MX.Application.Orders.DTOs
{
    // 🔹 Configuración y Asignación (Admin)
    public record ConfigureScheduleDto(DayOfWeek Day, TimeSpan StartTime, TimeSpan EndTime);
    public record AssignVisitDto(Guid OperatorId, Guid? ClientId, Guid? ChainId, DateTime ScheduledDate);

    // 🔹 Creación y Edición de Órdenes
    public record CreateOrderDto(Guid OperatorId, Guid? ClientId, Guid? ChainId, List<OrderItemDto> Items)
    {
        public Guid WarehouseId { get; set; }
    }

    public record EditOrderDto(List<OrderItemDto> Items); // Solo el Admin edita los items
    public record OrderItemDto(Guid ProductId, int Quantity);

    // 🔹 Estados de Visita
    public record UpdateVisitStateDto(VisitState State, string? ReportNotes);

    // 🔹 Respuestas y Dashboards
    public record OrderResponseDto(Guid Id, decimal TotalAmount, OrderStatus Status, string? TicketPath, DateTime CreatedAt, List<string> Alerts);

    public record OperatorDashboardDto(
        List<VisitDto> Pending,
        List<VisitDto> Offline,
        List<VisitDto> NoResponse,
        List<OrderDto> Finalized
    );

    public record AdminDashboardDto(
        List<OrderDto> AllOrders,
        int TotalPending,
        int TotalFinalized,
        int TotalCancelled
    );

    public record VisitDto(Guid VisitId, Guid? ClientId, Guid? ChainId, VisitState State, DateTime ScheduledDate);
    public record OrderDto(Guid Id, Guid OperatorId, Guid? ClientId, Guid? ChainId, decimal TotalAmount, OrderStatus Status, DateTime CreatedAt);
}
