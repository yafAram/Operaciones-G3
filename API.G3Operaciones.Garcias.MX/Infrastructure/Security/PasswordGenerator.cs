using API.G3Operaciones.Garcias.MX.Application.Common.Security;
using System.Security.Cryptography;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Security;

public sealed class PasswordGenerator : IPasswordGenerator
{
    private const string Characters =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";

    public string Generate(int length = 16)
    {
        if (length < 12 || length > 256)
            throw new ArgumentOutOfRangeException(nameof(length), "La longitud debe estar entre 12 y 256 caracteres.");

        return string.Create(length, Characters, static (buffer, characters) =>
        {
            for (var index = 0; index < buffer.Length; index++)
            {
                buffer[index] = characters[RandomNumberGenerator.GetInt32(characters.Length)];
            }
        });
    }
}
