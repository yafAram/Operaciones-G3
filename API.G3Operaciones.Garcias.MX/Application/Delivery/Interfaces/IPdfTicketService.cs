namespace API.G3Operaciones.Garcias.MX.Application.Delivery.Interfaces
{
    public interface IPdfTicketService
    {
        // Genera ticket de entrega
        Task<string> GenerateDeliveryTicketAsync(Guid deliveryStopId);
    }
}
