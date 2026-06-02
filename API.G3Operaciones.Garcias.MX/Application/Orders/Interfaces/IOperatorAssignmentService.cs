using API.G3Operaciones.Garcias.MX.Application.Orders.DTOs;

namespace API.G3Operaciones.Garcias.MX.Application.Orders.Interfaces
{
    public interface IOperatorAssignmentService
    {
        Task AssignTargetsAsync(AssignOperatorTargetsDto dto, string adminUserId);
        Task<List<Guid>> GetAssignedClientIdsAsync(Guid operatorId);
    }
}
