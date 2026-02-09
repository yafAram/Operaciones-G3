using API.G3Operaciones.Garcias.MX.Application.Users.Interfaces;

namespace API.G3Operaciones.Garcias.MX.Application.Users.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string password, string hash)
            => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
