using API.G3Operaciones.Garcias.MX.Application.Auth.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Auth.Services;
using API.G3Operaciones.Garcias.MX.Application.Chains.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Chains.Services;
using API.G3Operaciones.Garcias.MX.Application.Clients.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Clients.Services;
using API.G3Operaciones.Garcias.MX.Application.Common.Mappings;
using API.G3Operaciones.Garcias.MX.Application.Common.Security;
using API.G3Operaciones.Garcias.MX.Application.Delivery.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Delivery.Services;
using API.G3Operaciones.Garcias.MX.Application.Inventory.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Inventory.Services;
using API.G3Operaciones.Garcias.MX.Application.Orders.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Orders.Services;
using API.G3Operaciones.Garcias.MX.Application.Pricing.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Pricing.Services;
using API.G3Operaciones.Garcias.MX.Application.Products.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Products.Services;
using API.G3Operaciones.Garcias.MX.Application.Users.Interfaces;
using API.G3Operaciones.Garcias.MX.Application.Users.Services;
using API.G3Operaciones.Garcias.MX.Infrastructure;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence;
using API.G3Operaciones.Garcias.MX.Infrastructure.Persistence.Configurations;
using API.G3Operaciones.Garcias.MX.Infrastructure.Security;
using API.G3Operaciones.Garcias.MX.Infrastructure.Services;
using API.G3Operaciones.Garcias.MX.Middlewares;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
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
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();
builder.Services.AddScoped<IRoleCommandService, RoleCommandService>();
builder.Services.AddScoped<IRoleQueryService, RoleQueryService>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPasswordGenerator, PasswordGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IChainService, ChainService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IPriceConfigurationService, PriceConfigurationService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IPdfTicketService, PdfTicketService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IRouteOptimizationService, RouteOptimizationService>();
builder.Services.AddScoped<IGoogleMapsService, GoogleMapsService>();
builder.Services.AddScoped<IOrderStatusUpdater, OrderService>();
builder.Services.AddScoped<IOperatorAssignmentService, OrderService>();




//Mapping
builder.Services.AddAutoMapper(cfg =>
{
}, typeof(ClientProfile).Assembly);

//LOGS
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);




// JWT Configuration
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();



builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
        new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
});


//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.InvalidModelStateResponseFactory = context =>
//    {
//        return new BadRequestObjectResult(new
//        {
//            error = "Datos de inicio de sesión inválidos"
//        });
//    };
//});





// Fail fast when required deployment configuration is missing.
var jwtSecret = builder.Configuration["JwtSettings:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
{
    throw new InvalidOperationException(
        "JwtSettings:Secret must be configured with a secret of at least 32 bytes.");
}

if (string.IsNullOrWhiteSpace(builder.Configuration["JwtSettings:Issuer"]) ||
    string.IsNullOrWhiteSpace(builder.Configuration["JwtSettings:Audience"]))
{
    throw new InvalidOperationException("JWT issuer and audience must be configured.");
}

if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
{
    throw new InvalidOperationException("ConnectionStrings:DefaultConnection must be configured.");
}

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

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy =>
            policy.RequireRole("Administrador"));
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// Limite de tamaño de subida (10MB)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
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
    // Aplica migraciones pendientes
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }

    // Inicializa únicamente roles de referencia; no crea cuentas ni credenciales.
    await DataSeeder.SeedAsync(dbContext);
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

app.UseCors("FrontendPolicy");

// Autorización 
app.UseAuthentication();
app.UseAuthorization();

//Imagenes
// Habilitar archivos estáticos
app.UseStaticFiles();



app.MapControllers();

app.Run();
