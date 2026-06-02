using API.G3Operaciones.Garcias.MX.Application.Delivery.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Delivery.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Users.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/delivery/admin")]
    [Authorize(Policy = "AdminOnly")]
    public class DeliveryAdminController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveryAdminController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        // =========================
        // PLAN
        // =========================

        [HttpPost("plans")]
        public async Task<IActionResult> CreatePlan([FromBody] CreateDeliveryAssignmentPlanDto dto)
        {
            var id = await _deliveryService.CreatePlanAsync(dto);
            return Ok(new { id });
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans([FromQuery] DateTime date)
        {
            return Ok(await _deliveryService.GetPlansAsync(date));
        }

        [HttpGet("plans/{planId:guid}")]
        public async Task<IActionResult> GetPlanDetail(Guid planId)
        {
            return Ok(await _deliveryService.GetPlanDetailAsync(planId));
        }

        [HttpDelete("plans/{planId:guid}")]
        public async Task<IActionResult> DeletePlan(Guid planId)
        {
            await _deliveryService.DeletePlanAsync(planId);
            return NoContent();
        }

        // =========================
        // TARGETS
        // =========================

        [HttpPost("plans/{planId:guid}/targets")]
        public async Task<IActionResult> AddTarget(Guid planId, [FromBody] AddDeliveryAssignmentTargetDto dto)
        {
            var id = await _deliveryService.AddTargetAsync(planId, dto);
            return Ok(new { id });
        }

        [HttpDelete("targets/{targetId:guid}")]
        public async Task<IActionResult> RemoveTarget(Guid targetId)
        {
            await _deliveryService.RemoveTargetAsync(targetId);
            return NoContent();
        }

        [HttpPost("plans/{planId:guid}/confirm")]
        public async Task<IActionResult> ConfirmPlan(Guid planId)
        {
            await _deliveryService.ConfirmPlanAsync(planId);
            return NoContent();
        }

        // =========================
        // ROUTING
        // =========================

        [HttpPost("routes/generate")]
        public async Task<IActionResult> GenerateRoute([FromBody] GenerateRouteDto dto)
        {
            return Ok(await _deliveryService.GenerateRouteAsync(dto));
        }

        [HttpGet("routes")]
        public async Task<IActionResult> GetRoutes([FromQuery] DateTime date)
        {
            return Ok(await _deliveryService.GetRoutesAsync(date));
        }

        [HttpPost("routes/{planId:guid}/reassign")]
        public async Task<IActionResult> ReassignDriver(Guid planId, [FromQuery] Guid userId)
        {
            await _deliveryService.ReassignDriverAsync(planId, userId);
            return NoContent();
        }

        [HttpDelete("routes/{planId:guid}")]
        public async Task<IActionResult> CancelRoute(Guid planId)
        {
            await _deliveryService.CancelRouteAsync(planId);
            return NoContent();
        }

        // =========================
        // DASHBOARD
        // =========================

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview([FromQuery] DateTime date)
        {
            return Ok(await _deliveryService.GetDeliveryOverviewAsync(date));
        }

        [HttpGet("drivers")]
        public async Task<IActionResult> GetDrivers()
        {
            return Ok(await _deliveryService.GetDeliveryUsersAsync());
        }
    }
}