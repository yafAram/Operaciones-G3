namespace API.G3Operaciones.Garcias.MX.Application.Orders.Interfaces
{
    public interface IAuditService
    {
        /// <summary>
        /// Registra una acción en la bitácora de auditoría.
        /// </summary>
        /// <param name="entityName">Nombre de la entidad afectada (ej. Order, ClientVisit).</param>
        /// <param name="entityId">Identificador único de la entidad.</param>
        /// <param name="action">Acción realizada (ej. Create, Edit, Cancel, UpdateState).</param>
        /// <param name="changes">Objeto con los datos del cambio (se serializará a JSON internamente).</param>
        /// <param name="userId">Identificador del usuario (Admin u Operador) que realizó la acción.</param>
        Task LogActionAsync(string entityName, Guid entityId, string action, object changes, string userId);
    }
}
