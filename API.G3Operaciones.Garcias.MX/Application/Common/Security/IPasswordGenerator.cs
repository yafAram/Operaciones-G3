namespace API.G3Operaciones.Garcias.MX.Application.Common.Security;

public interface IPasswordGenerator
{
    string Generate(int length = 16);
}
