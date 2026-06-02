using API.G3Operaciones.Garcias.MX.Application.Orders.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Orders.Interfaces
{
    public interface IOrderService
    {
        Task ConfigureScheduleAsync(ConfigureScheduleDto dto, string adminUserId);
        Task<Guid> AssignVisitAsync(AssignVisitDto dto, string adminUserId);

        Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto request, string adminUserId);
        Task<OrderResponseDto> EditOrderAsync(Guid orderId, EditOrderDto dto, string adminUserId);
        Task CancelOrderAsync(Guid orderId, string adminUserId);

        Task UpdateVisitStateAsync(Guid visitId, UpdateVisitStateDto dto, string currentUserId);

        Task<OperatorDashboardDto> GetOperatorDashboardAsync(Guid operatorId, DateTime date);
        Task<AdminDashboardDto> GetAdminDashboardAsync(DateTime date);
        Task<OrderResponseDto> GetOrderDetailsAsync(Guid orderId);

        Task<List<OrderChainGroupDto>> GetAllGroupedOrdersAsync(DateTime? date = null);
        Task<LogisticsViewDto> GetLogisticsViewAsync();
    }
}
