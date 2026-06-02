namespace API.G3Operaciones.Garcias.MX.Application.Orders.DTOs
{
    public class LogisticsViewDto
    {
        public List<LogisticsOperatorDto> Operators { get; set; } = new();
        public List<LogisticsChainDto> Chains { get; set; } = new();
    }
}
