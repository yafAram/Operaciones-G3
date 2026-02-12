namespace API.G3Operaciones.Garcias.MX.Application.Common.Security
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }

}
