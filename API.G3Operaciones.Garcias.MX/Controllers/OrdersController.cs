using API.G3Operaciones.Garcias.MX.Application.Orders.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Orders.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOperatorAssignmentService _assignmentService;

        public OrdersController(
            IOrderService orderService,
            IOperatorAssignmentService assignmentService)
        {
            _orderService = orderService;
            _assignmentService = assignmentService;
        }

        // ==========================================
        // 🔥 ADMIN - ASIGNACIÓN DE OPERADORES
        // ==========================================
        [HttpPost("operator-assignment")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AssignOperatorTargets([FromBody] AssignOperatorTargetsDto dto)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Admin";
            await _assignmentService.AssignTargetsAsync(dto, adminId);
            return Ok(new { message = "Asignación guardada correctamente" });
        }

        [HttpGet("operator-assignment/{operatorId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetOperatorAssignments(Guid operatorId)
        {
            var clients = await _assignmentService.GetAssignedClientIdsAsync(operatorId);
            return Ok(new { OperatorId = operatorId, ClientIds = clients });
        }

        // ==========================================
        // 🔹 ÓRDENES (CORE)
        // ==========================================
        [HttpPost]
        [Authorize(Roles = "Administrador,Operador")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "User";
            var result = await _orderService.CreateOrderAsync(dto, userId);
            return Ok(result);
        }

        [HttpPut("{id}/edit")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> EditOrder(Guid id, [FromBody] EditOrderDto dto)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Admin";
            var result = await _orderService.EditOrderAsync(id, dto, adminId);
            return Ok(result);
        }

        [HttpPut("{id}/cancel")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Admin";
            await _orderService.CancelOrderAsync(id, adminId);
            return NoContent();
        }

        [HttpPut("{id}/deliver")]
        [Authorize(Roles = "Administrador,Operador")]
        public async Task<IActionResult> MarkAsDelivered(Guid id)
        {
            var statusUpdater = _orderService as IOrderStatusUpdater;
            if (statusUpdater != null)
            {
                await statusUpdater.MarkAsDeliveredAsync(id);
                return Ok(new { message = "Orden marcada como entregada." });
            }
            return BadRequest("El servicio no implementa la actualización de estados.");
        }

        [HttpPut("{id}/incident")]
        [Authorize(Roles = "Administrador,Operador")]
        public async Task<IActionResult> MarkAsIncident(Guid id)
        {
            var statusUpdater = _orderService as IOrderStatusUpdater;
            if (statusUpdater != null)
            {
                await statusUpdater.MarkAsIncidentAsync(id);
                return Ok(new { message = "Orden marcada con incidencia." });
            }
            return BadRequest("El servicio no implementa la actualización de estados.");
        }

        // ==========================================
        // 📊 DASHBOARDS Y LOGÍSTICA
        // ==========================================
        [HttpGet("dashboard/admin")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAdminDashboard([FromQuery] DateTime date)
        {
            var result = await _orderService.GetAdminDashboardAsync(date);
            return Ok(result);
        }

        [HttpGet("dashboard/operator/{operatorId}")]
        [Authorize(Roles = "Administrador,Operador")]
        public async Task<IActionResult> GetOperatorDashboard(Guid operatorId, [FromQuery] DateTime date)
        {
            var result = await _orderService.GetOperatorDashboardAsync(operatorId, date);
            return Ok(result);
        }

        [HttpGet("grouped")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetGroupedOrders([FromQuery] DateTime? date = null)
        {
            var result = await _orderService.GetAllGroupedOrdersAsync(date);
            return Ok(result);
        }

        [HttpGet("logistics")]
        [Authorize(Roles = "Administrador,Operador")] // Permite al Operador ver sus sucursales
        public async Task<IActionResult> GetLogisticsView()
        {
            var result = await _orderService.GetLogisticsViewAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Operador")]
        public async Task<IActionResult> GetOrderDetails(Guid id)
        {
            var result = await _orderService.GetOrderDetailsAsync(id);
            return Ok(result);
        }
    }
}