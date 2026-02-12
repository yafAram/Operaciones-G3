using API.G3Operaciones.Garcias.MX.Application.Roles.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Users.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/roles")]
    [Authorize(Policy = "AdminOnly")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleCommandService _roleCommandService;
        private readonly IRoleQueryService _roleQueryService;

        public RolesController(
            IRoleCommandService roleCommandService,
            IRoleQueryService roleQueryService)
        {
            _roleCommandService = roleCommandService;
            _roleQueryService = roleQueryService;
        }

        // 🔹 Listar roles
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleQueryService.GetAllAsync();
            return Ok(roles);
        }

        // 🔹 Crear rol
        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleDto dto)
        {
            var id = await _roleCommandService.CreateAsync(dto);
            return Ok(id);
        }

        // 🔹 Editar rol
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateRoleDto dto)
        {
            await _roleCommandService.UpdateAsync(id, dto);
            return NoContent();
        }

        // 🔹 Activar / Desactivar rol
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] bool activate)
        {
            await _roleCommandService.ChangeStatusAsync(id, activate);
            return NoContent();
        }
    }
}
