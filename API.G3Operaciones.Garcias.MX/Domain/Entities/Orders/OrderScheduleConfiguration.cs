namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Orders
{
    public class OrderScheduleConfiguration
    {
        public Guid Id { get; private set; }

        public DayOfWeek OrderDay { get; private set; }
        public DayOfWeek DeliveryDay { get; private set; }

        // Ventana en horas (default 38)
        public int AllowedHoursWindow { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        protected OrderScheduleConfiguration() { }

        public OrderScheduleConfiguration(
            DayOfWeek orderDay,
            DayOfWeek deliveryDay,
            int allowedHoursWindow = 38)
        {
            Id = Guid.NewGuid();
            OrderDay = orderDay;
            DeliveryDay = deliveryDay;
            AllowedHoursWindow = allowedHoursWindow;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Deactivate() => IsActive = false;
    }
}
