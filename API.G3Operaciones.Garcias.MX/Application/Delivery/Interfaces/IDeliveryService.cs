using API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs;

public interface IDeliveryService
{
    // ADMIN
    Task<Guid> CreatePlanAsync(CreateDeliveryAssignmentPlanDto dto);
    Task<List<DeliveryPlanSummaryDto>> GetPlansAsync(DateTime date);
    Task<DeliveryPlanDetailDto> GetPlanDetailAsync(Guid planId);
    Task DeletePlanAsync(Guid planId);

    Task<Guid> AddTargetAsync(Guid? planId, AddDeliveryAssignmentTargetDto dto);
    Task RemoveTargetAsync(Guid targetId);

    Task<DeliveryRouteResponseDto> GenerateRouteAsync(GenerateRouteDto dto);
    Task<List<RouteResponseDto>> GetRoutesAsync(DateTime date);
    Task<List<DeliveryAssignmentOverviewDto>> GetDeliveryOverviewAsync(DateTime date);

    Task<List<DeliveryUserResponseDto>> GetDeliveryUsersAsync();

    Task ReassignDriverAsync(Guid deliveryPlanId, Guid deliveryUserId);
    Task CancelRouteAsync(Guid deliveryPlanId);

    // OPERADOR
    Task<DeliveryRouteResponseDto> GetCurrentRouteAsync(Guid deliveryUserId, DateTime date);
    Task<DeliveryRouteResponseDto> StartRouteAsync(StartRouteDto dto);
    Task CompleteDeliveryAsync(CompleteDeliveryDto dto, string userId);
    Task ReportIncidentAsync(ReportIncidentDto dto, string userId);
    Task ConfirmPlanAsync(Guid planId);
}