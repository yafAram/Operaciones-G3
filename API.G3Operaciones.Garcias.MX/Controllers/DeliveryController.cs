using API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Delivery.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/delivery/operator")]
    [Authorize(Roles = "Repartidor")]
    public class DeliveryOperatorController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;
        private readonly IPdfTicketService _pdfTicketService;
        private readonly ILogger<DeliveryOperatorController> _logger;

        public DeliveryOperatorController(
            IDeliveryService deliveryService,
            IPdfTicketService pdfTicketService,
            ILogger<DeliveryOperatorController> logger)
        {
            _deliveryService = deliveryService;
            _pdfTicketService = pdfTicketService;
            _logger = logger;
        }

        // =========================
        // 🔹 PREVIEW (ANTES DE INICIAR)
        // =========================
        [HttpGet("route/preview")]
        public async Task<IActionResult> GetRoutePreview([FromQuery] DateTime date)
        {
            var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdRaw))
                return Unauthorized("UserId no encontrado");

            var userId = Guid.Parse(userIdRaw);

            var result = await _deliveryService.GetCurrentRouteAsync(userId, date);

            var isActive = result.DeliveryPlanId != Guid.Empty;

            return Ok(new
            {
                type = isActive ? "active" : "preview",
                isActive,
                stopsCount = result.Stops?.Count ?? 0,
                data = result
            });
        }

        // =========================
        // 🔹 RUTA ACTIVA
        // =========================
        [HttpGet("route/current")]
        public async Task<IActionResult> GetCurrentRoute([FromQuery] DateTime date)
        {
            var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdRaw))
                return Unauthorized("UserId no encontrado");

            var userId = Guid.Parse(userIdRaw);

            _logger.LogInformation("🚚 CURRENT ROUTE | UserId: {UserId} | Date: {Date}", userId, date);

            var result = await _deliveryService.GetCurrentRouteAsync(userId, date);

            if (result.DeliveryPlanId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ No hay ruta iniciada");
                return BadRequest("No has iniciado ruta. Usa /route/preview");
            }

            return Ok(new
            {
                type = "active",
                planId = result.DeliveryPlanId,
                stopsCount = result.Stops?.Count ?? 0,
                data = result
            });
        }


        [HttpPost("plans/{planId:guid}/confirm")]
        public async Task<IActionResult> ConfirmPlan(Guid planId)
        {
            await _deliveryService.ConfirmPlanAsync(planId);
            return NoContent();
        }

        [HttpPost("route/start")]
        public async Task<IActionResult> StartRoute([FromBody] StartRouteDto dto)
        {
            var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdRaw))
                return Unauthorized("UserId no encontrado");

            dto.DeliveryUserId = Guid.Parse(userIdRaw);

            _logger.LogInformation(
                "🚀 START ROUTE | UserId: {UserId} | Lat: {Lat} | Lon: {Lon}",
                dto.DeliveryUserId, dto.CurrentLatitude, dto.CurrentLongitude);

            var result = await _deliveryService.StartRouteAsync(dto);

            return Ok(new
            {
                type = "active",
                message = "Ruta iniciada y optimizada correctamente",
                data = result
            });
        }

        // =========================
        // 🔹 COMPLETAR ENTREGA
        // =========================
        [HttpPost("delivery/complete")]
        public async Task<IActionResult> CompleteDelivery([FromBody] CompleteDeliveryDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            _logger.LogInformation("📦 COMPLETE | StopId: {StopId} | UserId: {UserId}",
                dto.DeliveryStopId,
                userId);

            await _deliveryService.CompleteDeliveryAsync(dto, userId);

            return Ok(new
            {
                message = "Entrega completada",
                stopId = dto.DeliveryStopId
            });
        }

        // =========================
        // 🔹 TICKET PDF ENTREGA
        // =========================
        [HttpGet("delivery/ticket/{deliveryStopId:guid}")]
        public async Task<IActionResult> GenerateDeliveryTicket(Guid deliveryStopId)
        {
            var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdRaw))
                return Unauthorized("UserId no encontrado");

            _logger.LogInformation(
                "🧾 TICKET | StopId: {StopId} | UserId: {UserId}",
                deliveryStopId,
                userIdRaw);

            try
            {
                var relativePath = await _pdfTicketService.GenerateDeliveryTicketAsync(deliveryStopId);

                return Ok(new
                {
                    message = "Ticket generado correctamente",
                    deliveryStopId,
                    url = relativePath
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("❌ TICKET NOT FOUND | {Message}", ex.Message);

                return NotFound(new
                {
                    message = ex.Message,
                    deliveryStopId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERROR GENERANDO TICKET");

                return StatusCode(500, new
                {
                    message = "Error al generar ticket",
                    detail = ex.Message
                });
            }
        }

        // =========================
        // 🔹 REPORTAR INCIDENTE
        // =========================
        [HttpPost("delivery/incident")]
        public async Task<IActionResult> ReportIncident([FromBody] ReportIncidentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            _logger.LogInformation("⚠️ INCIDENT | StopId: {StopId} | UserId: {UserId} | Notes: {Notes}",
                dto.DeliveryStopId,
                userId,
                dto.Notes);

            await _deliveryService.ReportIncidentAsync(dto, userId);

            return Ok(new
            {
                message = "Incidencia reportada",
                stopId = dto.DeliveryStopId
            });
        }
    }
}