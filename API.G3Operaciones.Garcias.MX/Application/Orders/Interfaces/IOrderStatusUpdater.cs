namespace API.G3Operaciones.Garcias.MX.Application.Orders.Interfaces
{
    public interface IOrderStatusUpdater
    {
        Task MarkAsDeliveredAsync(Guid orderId);
        Task MarkAsIncidentAsync(Guid orderId);
    }
}