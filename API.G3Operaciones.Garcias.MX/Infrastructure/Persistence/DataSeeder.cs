using API.G3Operaciones.Garcias.MX.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;

/// <summary>
/// Seeds only non-sensitive reference data. Administrative accounts must be
/// provisioned through a controlled process and never through source-code defaults.
/// </summary>
public static class DataSeeder
{
    private static readonly string[] RequiredRoles =
    [
        "Administrador",
        "Operador",
        "Repartidor"
    ];

    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        foreach (var roleName in RequiredRoles)
        {
            var exists = await context.Roles
                .AnyAsync(role => role.Name == roleName, cancellationToken);

            if (!exists)
            {
                context.Roles.Add(new Role(roleName));
            }
        }

        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
