using API.G3Operaciones.Garcias.MX.Domain.Entities.Users;

namespace API.G3Operaciones.Garcias.MX.Application.Auth.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, bool mustChangePassword);
    }
}
