namespace API.G3Operaciones.Garcias.MX.Application.Orders.DTOs
{
    public class AssignOperatorTargetsDto
    {
        public Guid OperatorId { get; set; }
        public List<Guid>? ClientIds { get; set; }
        public List<Guid>? ChainIds { get; set; }
    }
}
