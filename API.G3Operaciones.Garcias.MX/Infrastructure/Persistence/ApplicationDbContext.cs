using API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Clients;
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
        public DbSet<OrderHistory> OrderHistories => Set<OrderHistory>();
        public DbSet<OrderCallLog> OrderCallLogs => Set<OrderCallLog>();
        public DbSet<OperatorAssignment> OperatorAssignments => Set<OperatorAssignment>();
        public DbSet<OrderScheduleConfiguration> OrderScheduleConfigurations => Set<OrderScheduleConfiguration>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica automáticamente todas las configuraciones
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }
    }
}