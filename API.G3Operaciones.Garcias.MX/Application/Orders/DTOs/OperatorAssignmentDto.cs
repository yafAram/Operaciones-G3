namespace API.G3Operaciones.Garcias.MX.Application.Orders.DTOs
{
    public class OperatorAssignmentDto
    {
        public int Id { get; set; }

        public int OperatorId { get; set; }

        public int BranchId { get; set; }

        public string BranchName { get; set; } = string.Empty;

        public DayOfWeek AssignedDay { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
