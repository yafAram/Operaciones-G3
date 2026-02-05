using API.G3Operaciones.Garcias.MX.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// Add services to the container
// =====================================================

// Controllers (API REST)
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure
// Aquí se registra:
// - DbContext (SQL Server)
// - Servicios técnicos (futuro: JWT, WhatsApp, etc)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// =====================================================
// Database initialization (MIGRACIONES AUTOMÁTICAS)
// =====================================================

/*
 * ESTE BLOQUE ES OPCIONAL Y ACTUALMENTE COMENTADO A PROPÓSITO.
 *
 * ¿Qué hace?
 * - Al iniciar la aplicación:
 *   - Verifica si existen migraciones pendientes
 *   - Si NO existen → no hace nada
 *   - Si existen → las aplica automáticamente
 *
 * ¿Por qué usarlo?
 * - Ideal para ambientes controlados (DEV / QA)
 * - Evita ejecutar migraciones manualmente
 *
 * ¿Por qué está comentado ahora?
 * - Aún no existen entidades ni migraciones
 * - Se activará cuando el modelo de dominio esté definido
 *
 * ⚠️ En PRODUCCIÓN debe usarse con criterio
 */

// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//
//     if (dbContext.Database.GetPendingMigrations().Any())
//     {
//         dbContext.Database.Migrate();
//     }
// }

// =====================================================
// HTTP request pipeline
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Autorización (JWT vendrá después)
app.UseAuthorization();

app.MapControllers();

app.Run();
