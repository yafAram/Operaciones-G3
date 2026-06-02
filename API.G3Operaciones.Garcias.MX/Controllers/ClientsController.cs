using API.G3Operaciones.Garcias.MX.Application.Clients.Dto;
using API.G3Operaciones.Garcias.MX.Application.Clients.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/clients")]
    [Authorize(Policy = "AdminOnly")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _service;

        public ClientsController(IClientService service)
        {
            _service = service;
        }

        // =====================================================
        // Crear cliente (Sucursal)
        // Solo Administrador
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientDto dto)
        {
            var result = await _service.CreateAsync(dto);
            //return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            return StatusCode(201, result);

        }

        // =====================================================
        // Actualizar cliente
        // Solo Administrador
        // =====================================================
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        // =====================================================
        // Obtener todos
        // Admin y Operador pueden consultar
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }



        // =====================================================
        // Desactivar cliente
        // =====================================================

        [HttpPatch("{id:guid}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _service.DeactivateAsync(id);
            return NoContent();
        }

        // =====================================================
        // Activar cliente
        // =====================================================

        [HttpPatch("{id:guid}/activate")]
        public async Task<IActionResult> Activate(Guid id)
        {
            await _service.ActivateAsync(id);
            return NoContent();
        }


        //// =====================================================
        //// Obtener por Id
        //// =====================================================
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetDetailByIdAsync(id);
            return Ok(result);
        }

    }
}
