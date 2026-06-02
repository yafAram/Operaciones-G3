using API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Delivery.Interfaces;

namespace API.G3Operaciones.Garcias.MX.Application.Delivery.Services
{
    public class GoogleMapsService : IGoogleMapsService
    {
        // Genera links por bloques de 8 paradas
        public List<string> GenerateRouteLinks(
            decimal originLat,
            decimal originLng,
            List<DeliveryStopDto> stops)
        {
            var result = new List<string>();

            var groups = stops
                .GroupBy(x => x.RouteGroup)
                .OrderBy(g => g.Key);

            foreach (var group in groups)
            {
                var ordered = group.OrderBy(x => x.Sequence).ToList();

                var origin = $"{originLat},{originLng}";
                var destination = $"{ordered.Last().Latitude},{ordered.Last().Longitude}";

                var waypoints = string.Join("|",
                    ordered.Take(ordered.Count - 1)
                    .Select(x => $"{x.Latitude},{x.Longitude}")
                );

                var url = $"https://www.google.com/maps/dir/?api=1&origin={origin}&destination={destination}&waypoints={waypoints}";

                result.Add(url);
            }

            return result;
        }
    }
}
