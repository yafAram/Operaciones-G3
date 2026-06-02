namespace API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs
{
    public record RouteResponseDto(
    Guid Id,
    Guid DeliveryUserId,
    string DeliveryUserName,
    DateTime Date,
    string Status,
    int TotalStops,
    int DeliveredStops,
    int FailedStops,
    int PendingStops
);
}
