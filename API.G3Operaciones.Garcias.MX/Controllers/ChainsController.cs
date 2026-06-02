using API.G3Operaciones.Garcias.MX.Application.Chains.Dto;
using API.G3Operaciones.Garcias.MX.Application.Chains.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/chains")]
    [Authorize(Roles = "Administrador")]
    public class ChainsController : ControllerBase
    {
        private readonly IChainService _chainService;

        public ChainsController(IChainService chainService)
        {
            _chainService = chainService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateChainDto dto)
        {
            var result = await _chainService.CreateAsync(dto);
            return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
        }



        // =====================================================
        // Obtener todos
        // Admin y Operador pueden consultar
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _chainService.GetAllAsync();
            return Ok(result);
        }



        //// =====================================================
        //// Obtener por Id
        //// =====================================================
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _chainService.GetDetailByIdAsync(id);
            return Ok(result);
        }



        // =====================================================
        // Desactivar cliente
        // =====================================================

        [HttpPatch("{id:guid}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _chainService.DeactivateAsync(id);
            return NoContent();
        }


    }
}
