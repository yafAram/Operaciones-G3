using API.G3Operaciones.Garcias.MX.Application.Common.Security;
using API.G3Operaciones.Garcias.MX.Application.Common.Validation;
using API.G3Operaciones.Garcias.MX.Domain.Entities;
using API.G3Operaciones.Garcias.MX.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;

/// <summary>
/// Seeds reference roles and optionally provisions a first administrator from
/// deployment secrets. It never contains default usernames or passwords.
/// </summary>
public static class DataSeeder
{
    private static readonly string[] RequiredRoles =
    [
        "Administrador",
        "Operador",
        "Repartidor"
    ];

    public static async Task SeedAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        foreach (var roleName in RequiredRoles)
        {
            var exists = await context.Roles
                .AnyAsync(role => role.Name == roleName, cancellationToken);

            if (!exists)
                context.Roles.Add(new Role(roleName));
        }

        if (context.ChangeTracker.HasChanges())
            await context.SaveChangesAsync(cancellationToken);

        // Existing installations are never overwritten or re-seeded with an admin.
        if (await context.Users.AnyAsync(cancellationToken) ||
            !configuration.GetValue<bool>("BootstrapAdmin:Enabled"))
        {
            return;
        }

        var username = configuration["BootstrapAdmin:Username"];
        var password = configuration["BootstrapAdmin:Password"];
        var firstName = configuration["BootstrapAdmin:FirstName"];
        var lastName = configuration["BootstrapAdmin:LastName"];
        var birthDateValue = configuration["BootstrapAdmin:BirthDate"];

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) ||
            password.Length < 16 ||
            string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            !DateTime.TryParseExact(
                birthDateValue,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var birthDate))
        {
            throw new InvalidOperationException(
                "BootstrapAdmin is enabled but its username, password (minimum 16 characters), name and BirthDate (yyyy-MM-dd) are not configured correctly.");
        }

        if (!UsernameValidator.IsValid(username))
            throw new InvalidOperationException("BootstrapAdmin:Username has an invalid format.");

        var adminRole = await context.Roles
            .FirstAsync(role => role.Name == "Administrador", cancellationToken);

        var user = new User(
            firstName,
            lastName,
            birthDate,
            string.Empty,
            adminRole.Id);

        var credential = new UserCredential(
            user.Id,
            username,
            passwordHasher.Hash(password));

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        context.Users.Add(user);
        context.UserCredentials.Add(credential);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
