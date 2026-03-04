using API.G3Operaciones.Garcias.MX.Application.Pricing.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Pricing.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class PricingController : ControllerBase
    {
        private readonly IPriceConfigurationService _service;

        public PricingController(IPriceConfigurationService service)
        {
            _service = service;
        }

        // 🔹 Crear configuración de precio
        [HttpPost]
        public async Task<IActionResult> Create(CreatePriceConfigurationDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(new { Id = id });
        }

        // 🔹 Actualizar precio
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdatePriceConfigurationDto dto)
        {
            await _service.UpdatePriceAsync(id, dto);
            return NoContent();
        }

        // 🔹 Desactivar configuración
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _service.DeactivateAsync(id);
            return NoContent();
        }

        // 🔹 Obtener por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // 🔹 Obtener por producto
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId)
        {
            var result = await _service.GetByProductAsync(productId);
            return Ok(result);
        }

        // 🔹 Resolver precio (para pruebas)
        [HttpGet("resolve")]
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
