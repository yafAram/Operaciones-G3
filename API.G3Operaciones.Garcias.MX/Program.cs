using API.G3Operaciones.Garcias.MX.Application.Auth.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Auth.Services;
using API.G3Operaciones.Garcias.MX.Infrastructure;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations;
using API.G3Operaciones.Garcias.MX.Infrastructure.Services;
using API.G3Operaciones.Garcias.MX.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// Add services to the container
// =====================================================

// Controllers (API REST)
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// PRIMERO: Registra servicios de aplicación CORE
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>(); // ¡ESTA LÍNEA YA ESTÁ!
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();

// JWT Configuration
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured")
                )
            )
        };
    });

// LUEGO: Infrastructure (DbContext, Repositories, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// =====================================================
// Database initialization (MIGRACIONES AUTOMÁTICAS + SEED)
// =====================================================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

    // Aplica migraciones pendientes
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }

    // Crea usuario admin por defecto si no existe
    await DataSeeder.SeedAsync(dbContext, passwordHasher);
}

// =====================================================
// Middleware de excepciones
// =====================================================
app.UseMiddleware<ExceptionMiddleware>();

// =====================================================
// HTTP request pipeline
// =====================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Autorización 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
