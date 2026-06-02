using API.G3Operaciones.Garcias.MX.Application.Pricing.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Pricing.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // 🔥 ELIMINADO EL BLOQUEO A NIVEL DE CLASE AQUÍ
    public class PricingController : ControllerBase
    {
        private readonly IPriceConfigurationService _service;

        public PricingController(IPriceConfigurationService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")] // Solo Admin
        public async Task<IActionResult> Create(CreatePriceConfigurationDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(new { Id = id });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")] // Solo Admin
        public async Task<IActionResult> Update(Guid id, UpdatePriceConfigurationDto dto)
        {
            await _service.UpdatePriceAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")] // Solo Admin
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _service.DeactivateAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Operador")] // Permite leer
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("product/{productId}")]
        [Authorize(Roles = "Administrador,Operador")] // Permite leer al operador
        public async Task<IActionResult> GetByProduct(Guid productId)
        {
            var result = await _service.GetByProductAsync(productId);
            return Ok(result);
        }

        // 🔹 Resolver precio (Es el método más importante para la vista del carrito del operador)
        [HttpGet("resolve")]
        [Authorize(Roles = "Administrador,Operador")] // 🔥 ACCESO LIBERADO PARA EL OPERADOR
        public async Task<IActionResult> Resolve(
            Guid productId,
            Guid? chainId,
            Guid? clientId)
        {
            var price = await _service.ResolvePriceAsync(productId, chainId, clientId);
            return Ok(new { Price = price });
        }
    }
}