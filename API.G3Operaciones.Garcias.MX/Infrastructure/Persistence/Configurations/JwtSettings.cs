namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpirationHours { get; set; }
    }
}
