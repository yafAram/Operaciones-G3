using API.G3Operaciones.Garcias.MX.Application.Common.Security;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Security
{
    public class PasswordGenerator : IPasswordGenerator
    {
        public string Generate(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
