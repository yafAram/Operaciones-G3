using API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Clients;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Inventory;
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

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserCredential> UserCredentials => Set<UserCredential>();
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Chain> Chains => Set<Chain>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===============================
            // INVENTORY
            // ===============================

            modelBuilder.Entity<Inventory>()
                .HasIndex(i => new { i.ProductId, i.WarehouseId })
                .IsUnique();

            modelBuilder.Entity<Inventory>()
                .Property(i => i.Quantity)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Warehouse)
                .WithMany()
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);


            // ===============================
            // INVENTORY MOVEMENT
            // ===============================

            modelBuilder.Entity<InventoryMovement>()
                .Property(m => m.Quantity)
                .HasPrecision(18, 4);


            // ===============================
            // CLIENT → CHAIN
            // ===============================

            modelBuilder.Entity<Client>()
                .HasOne(c => c.Chain)
                .WithMany(ch => ch.Clients)
                .HasForeignKey(c => c.ChainId)
                .OnDelete(DeleteBehavior.Restrict);


            // ===============================
            // PRICE CONFIGURATION
            // ===============================

            modelBuilder.Entity<PriceConfiguration>()
                .Property(p => p.Price)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PriceConfiguration>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PriceConfiguration>()
                .HasOne(p => p.Chain)
                .WithMany()
                .HasForeignKey(p => p.ChainId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PriceConfiguration>()
                .HasOne(p => p.Client)
                .WithMany()
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice para evitar duplicados del mismo producto al mismo nivel
            modelBuilder.Entity<PriceConfiguration>()
                .HasIndex(p => new { p.ProductId, p.ChainId, p.ClientId })
                .IsUnique();


            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }

    }

}
