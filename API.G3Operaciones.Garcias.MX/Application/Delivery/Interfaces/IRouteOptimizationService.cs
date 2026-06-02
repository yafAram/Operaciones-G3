using API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Delivery.Interfaces
{
    public interface IRouteOptimizationService
    {
        // Ordena paradas en base a ubicación actual
        List<DeliveryStopDto> OptimizeRoute(
            decimal startLat,
            decimal startLng,
            List<DeliveryStopDto> stops);
    }
}
