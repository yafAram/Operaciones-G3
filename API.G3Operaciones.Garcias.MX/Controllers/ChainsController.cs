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
    }
}
