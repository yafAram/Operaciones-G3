using API.G3Operaciones.Garcias.MX.Application.Users.DTOs;
using API.G3Operaciones.Garcias.MX.Application.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Administrador")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRegistrationService _registrationService;

        public UsersController(IUserRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var result = await _registrationService.RegisterUserAsync(dto);

            if (!result.Success)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "Usuario creado correctamente" });
        }
    }

}
