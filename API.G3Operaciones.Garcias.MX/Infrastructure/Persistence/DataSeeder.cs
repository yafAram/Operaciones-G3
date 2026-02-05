using API.G3Operaciones.Garcias.MX.Application.Auth.Interfaces;
using API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            // Verifica si ya existen usuarios
            if (await context.Users.AnyAsync())
                return;

            // Crear rol administrador
            var adminRole = new Role
            {
                Name = "Administrador",
            };
            context.Roles.Add(adminRole);

            // Crear usuario administrador
            var adminUser = new User(
                firstName: "Admin",
                lastName: "Default",
                birthDate: DateTime.MinValue, // Ajusta según sea necesario
                phoneNumber: "", // Ajusta según sea necesario
                roleId: adminRole.Id
            );
            context.Users.Add(adminUser);

            // Crear credencial para login
            var adminCredential = new UserCredential(
                userId: adminUser.Id,
                username: "admin",
                passwordHash: passwordHasher.Hash("Admin123!") // contraseña inicial
            );
            context.Set<UserCredential>().Add(adminCredential);

            await context.SaveChangesAsync();
        }
    }
}
