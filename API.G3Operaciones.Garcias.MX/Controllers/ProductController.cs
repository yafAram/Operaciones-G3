using API.G3Operaciones.Garcias.MX.Application.Products.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Products.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        // ===============================
        // CREATE
        // ===============================
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return Ok(id);
        }

        // ===============================
        // UPDATE
        // ===============================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateProductDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        // ===============================
        // DEACTIVATE
        // ===============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _service.DeactivateAsync(id);
            return NoContent();
        }

        // ===============================
        // GET BY ID
        // ===============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _service.GetByIdAsync(id);
            return Ok(product);
        }

        // ===============================
        // GET ALL
        // ===============================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        // ===============================
        // TOP PRODUCTS
        // ===============================
        [HttpGet("top/{top}")]
        public async Task<IActionResult> GetTop(int top)
        {
            var result = await _service.GetTopProductsAsync(top);
            return Ok(result);
        }

        // ===============================
        // AJUSTAR STOCK
        // ===============================
        [HttpPost("stock/in")]
        public async Task<IActionResult> IncreaseStock([FromBody] AdjustStockDto dto)
        {
            await _service.AdjustStockAsync(dto, MovementType.In);
            return NoContent();
        }

        [HttpPost("stock/out")]
        public async Task<IActionResult> DecreaseStock([FromBody] AdjustStockDto dto)
        {
            await _service.AdjustStockAsync(dto, MovementType.Out);
            return NoContent();
        }
    }
}