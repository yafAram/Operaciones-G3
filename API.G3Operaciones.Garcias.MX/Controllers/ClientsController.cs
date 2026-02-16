using API.G3Operaciones.Garcias.MX.Application.Clients.Dto;
using API.G3Operaciones.Garcias.MX.Application.Clients.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/clients")]
    [Authorize(Roles = "Administrador")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClientDto dto)
        {
            var result = await _clientService.CreateAsync(dto);
            return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
        }
    }

}
