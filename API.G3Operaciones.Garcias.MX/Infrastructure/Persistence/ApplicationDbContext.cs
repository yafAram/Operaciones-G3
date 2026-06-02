using API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Clients;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Delivery;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Orders;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Pricing;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Products;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ===============================
        // USERS
        // ===============================
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserCredential> UserCredentials => Set<UserCredential>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        // ===============================
        // CLIENTS
        // ===============================
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Chain> Chains => Set<Chain>();

        // ===============================
        // PRODUCTS
        // ===============================
        public DbSet<Product> Products => Set<Product>();

        // ===============================
        // INVENTORY
        // ===============================
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();

        // ===============================
        // PRICING
        // ===============================
        public DbSet<PriceConfiguration> PriceConfigurations => Set<PriceConfiguration>();

        // ===============================
        // ORDERS
        // ===============================
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();


        // ===============================
        // DELIVERY
        // ===============================

        public DbSet<DeliveryPlan> DeliveryPlans => Set<DeliveryPlan>();
        public DbSet<DeliveryStop> DeliveryStops => Set<DeliveryStop>();
        public DbSet<DeliveryIncident> DeliveryIncidents => Set<DeliveryIncident>();
        public DbSet<DeliveryAssignmentPlan> DeliveryAssignmentPlans { get; set; }
        public DbSet<DeliveryAssignmentTarget> DeliveryAssignmentTargets { get; set; }
        public DbSet<OperatorAssignmentPlan> OperatorAssignmentPlans { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Model.GetEntityTypes()
    .SelectMany(t => t.GetProperties())
    .Where(p => p.Name == "OrderStatusSnapshot");

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");
                entity.HasKey(e => e.Id);
            });
            // Aplica automáticamente todas las configuraciones
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }
    }
}