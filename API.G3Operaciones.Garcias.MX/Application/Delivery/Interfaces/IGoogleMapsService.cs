using API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Delivery.Interfaces
{
    public interface IGoogleMapsService
    {
        // Genera links por bloques de 8 paradas
        List<string> GenerateRouteLinks(
            decimal originLat,
            decimal originLng,
            List<DeliveryStopDto> stops);
    }
}
