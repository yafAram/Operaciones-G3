using API.G3Operaciones.Garcias.MX.Application.Inventory.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Inventory.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // 🔥 ELIMINADO EL BLOQUEO A NIVEL DE CLASE AQUÍ
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _service;

        public WarehouseController(IWarehouseService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(CreateWarehouseDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Operador")] // El operador necesita ver el catálogo
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Operador")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var warehouse = await _service.GetByIdAsync(id);

            if (warehouse is null)
                return NotFound();

            return Ok(warehouse);
        }

        [HttpPut("{id}/deactivate")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _service.DeactivateAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/activate")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Activate(Guid id)
        {
            await _service.ActivateAsync(id);
            return NoContent();
        }
    }
}