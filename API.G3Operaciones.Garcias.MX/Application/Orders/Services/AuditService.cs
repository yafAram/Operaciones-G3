using API.G3Operaciones.Garcias.MX.Application.Orders.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.G3Operaciones.Garcias.MX.Application.Orders.Services
{
    public class AuditService : IAuditService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;
        // private readonly ApplicationDbContext _context; // Descomentar si vas a guardar en Base de Datos

        public AuditService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // 👉 ESTE ES EL MÉTODO QUE EXIGE TU INTERFAZ
        public async Task LogActionAsync(string entityName, Guid entityId, string action, object changes, string userId)
        {
            // 1. Serializar los cambios a JSON para guardarlos fácilmente
            var changesJson = JsonSerializer.Serialize(changes);

            // 2. Registrar la acción en la consola (Logs)
            _logger.LogInformation(
                "AUDITORÍA -> Acción: {Action} | Entidad: {EntityName} | ID: {EntityId} | Usuario: {UserId} | Cambios: {Changes}",
                action, entityName, entityId, userId, changesJson);

            // 3. SI TIENES UNA TABLA EN BASE DE DATOS PARA AUDITORÍAS (ej. AuditLogs), SE HARÍA ASÍ:
            /*
            var auditLog = new AuditLog 
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                Changes = changesJson,
                UserId = userId,
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
            */

            await Task.CompletedTask; // Borra esta línea si descomentas el código de la base de datos
        }

        // Método extra útil para sacar el ID del usuario del token
        public string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}