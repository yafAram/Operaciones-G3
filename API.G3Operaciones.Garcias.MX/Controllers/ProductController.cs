using API.G3Operaciones.Garcias.MX.Application.Products.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Products.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // 🔥 ELIMINADO EL BLOQUEO A NIVEL DE CLASE AQUÍ
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "AdminOnly")] // Solo Admin crea
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(id);
        }

        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "AdminOnly")] // Solo Admin edita
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _service.DeactivateAsync(id);
            return NoContent();
        }

        [HttpDelete("Activate/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Activate(Guid id)
        {
            await _service.ActivateAsync(id);
            return NoContent();
        }

        // 🔥 Permite a los operadores cargar su catálogo de productos
        [HttpGet]
        [Authorize(Roles = "Administrador,Operador")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Operador")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _service.GetByIdAsync(id);
            return Ok(product);
        }

        [HttpGet("cards")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetCards()
        {
            var result = await _service.GetCardsAsync();
            return Ok(result);
        }

        [HttpGet("top/{top}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetTop(int top)
        {
            var result = await _service.GetTopProductsAsync(top);
            return Ok(result);
        }

        [HttpPost("stock/in")]
        [Authorize(Policy = "AdminOnly")] // Movimientos manuales de stock solo Admin
        public async Task<IActionResult> IncreaseStock([FromBody] AdjustStockDto dto)
        {
            await _service.AdjustStockAsync(dto, MovementType.In);
            return NoContent();
        }

        [HttpPost("stock/out")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DecreaseStock([FromBody] AdjustStockDto dto)
        {
            await _service.AdjustStockAsync(dto, MovementType.Out);
            return NoContent();
        }
    }
}