using API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Delivery.Interfaces;

namespace API.G3Operaciones.Garcias.MX.Application.Delivery.Services
{
    public class RouteOptimizationService : IRouteOptimizationService
    {
        // Ordena paradas usando Nearest Neighbor
        public List<DeliveryStopDto> OptimizeRoute(
            decimal startLat,
            decimal startLng,
            List<DeliveryStopDto> stops)
        {
            var remaining = new List<DeliveryStopDto>(stops);
            var ordered = new List<DeliveryStopDto>();

            var currentLat = startLat;
            var currentLng = startLng;

            while (remaining.Any())
            {
                var nearest = remaining
                    .OrderBy(x => Distance(currentLat, currentLng, x.Latitude, x.Longitude))
                    .First();

                ordered.Add(nearest);

                currentLat = nearest.Latitude;
                currentLng = nearest.Longitude;

                remaining.Remove(nearest);
            }

            // Asignar secuencia y grupos de 8
            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].Sequence = i + 1;
                ordered[i].RouteGroup = i / 8;
            }

            return ordered;
        }

        // Distancia Haversine simplificada
        private double Distance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var R = 6371; // km

            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lon2 - lon1));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians((double)lat1)) *
                    Math.Cos(ToRadians((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private double ToRadians(double angle)
        {
            return angle * Math.PI / 180;
        }
    }
}
