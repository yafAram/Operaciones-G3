using API.G3Operaciones.Garcias.MX.Application.Users.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Users.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Policy = "AdminOnly")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRegistrationService _registrationService;
        private readonly IUserCommandService _commandService;
        private readonly IUserQueryService _queryService;

        public UsersController( IUserRegistrationService registrationService, IUserCommandService commandService,
        IUserQueryService queryService)
        {
            _registrationService = registrationService;
            _commandService = commandService;
            _queryService = queryService;
        }

        // Crear usuario
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var result = await _registrationService.RegisterUserAsync(dto);

            if (!result.Success)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "Usuario creado correctamente" });
        }

        // Listar usuarios
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _queryService.GetAllAsync();
            return Ok(users);
        }

        // Editar usuario
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto dto)
        {
            await _commandService.UpdateAsync(id, dto);
            return Ok(new { message = "Usuario actualizado correctamente" });
        }

        // Desactivar usuario
        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(Guid id)
        {
            await _commandService.DeactivateAsync(id);
            return Ok(new { message = "Usuario desactivado correctamente" });
        }

        // Recuperacion de Contraseña(user)
        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(Guid id)
        {
            var temporaryPassword = await _commandService.ResetPasswordAsync(id);

            return Ok(new
            {
                message = "Contraseña reseteada correctamente",
                temporaryPassword
            });
        }


    }
}
