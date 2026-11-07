using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace API.G3Operaciones.Garcias.MX.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public HealthController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Liveness endpoint for process/container health probes.
        /// Does not query the database or expose infrastructure details.
        /// </summary>
        [HttpGet("live")]
        public IActionResult CheckLiveness()
        {
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTimeOffset.UtcNow
            });
        }

        [HttpGet("db")]
        public async Task<IActionResult> CheckDatabase()
        {
            var canConnect = await _dbContext.Database.CanConnectAsync();

            if (!canConnect)
            {
                return StatusCode(500, "No se pudo conectar a la base de datos.");
            }

            return Ok("Conexión a la base de datos exitosa.");
        }
    }
}
