using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FiapProjetoGames.Application.Services;
using FiapProjetoGames.Domain.Repositories;
using FiapProjetoGames.Infrastructure.Data;
using FiapProjetoGames.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FiapProjetoGames.Application.Validation;
using FiapProjetoGames.Application.DTOs;
using FiapProjetoGames.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "FIAP Projeto Games API", 
        Version = "1.0",
        Description = "API para plataforma educacional de jogos da FIAP"
    });

    // Configuração de autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure Health Checks
builder.Services.AddHealthChecks();

// Configure CORS - Permitir tudo
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure DbContext - In-memory apenas
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("RailwayDatabase"));

// Configure JWT Authentication - Simplificado
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "fiap-projeto-games-super-secret-key-2024-with-256-bits-minimum-security";
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// Register Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IJogoRepository, JogoRepository>();
builder.Services.AddScoped<IBibliotecaJogoRepository, BibliotecaJogoRepository>();

// Register Application Services
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();
builder.Services.AddSingleton<IMetricsService, MetricsService>();

builder.Services.AddScoped<IUsuarioService>(provider =>
{
    var usuarioRepository = provider.GetRequiredService<IUsuarioRepository>();
    var logService = provider.GetRequiredService<ILogService>();
    return new UsuarioService(usuarioRepository, logService, jwtSecret, "FiapProjetoGames", "FiapProjetoGamesUsers", 24);
});
builder.Services.AddScoped<IJogoService, JogoService>();
builder.Services.AddScoped<IBibliotecaJogoService, BibliotecaJogoService>();

// Register Validators
builder.Services.AddScoped<IValidator<CadastroUsuarioDto>, CadastroUsuarioValidation>();
builder.Services.AddScoped<IValidator<LoginUsuarioDto>, LoginUsuarioValidation>();



// Configure Memory Cache
builder.Services.AddMemoryCache();

// Register Database Initialization Service
builder.Services.AddScoped<DatabaseInitializationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FIAP Projeto Games API v1.0");
    c.RoutePrefix = "swagger";
});

// Configure CORS
app.UseCors("AllowAll");

// Global Error Handling
app.UseMiddleware<FiapProjetoGames.API.Middleware.ErrorHandlingMiddleware>();

// Metrics Middleware
app.UseMiddleware<FiapProjetoGames.API.Middleware.MetricsMiddleware>();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Health Checks
app.MapHealthChecks("/health");

// Map Controllers
app.MapControllers();

// Initialize database - Simplificado
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializationService>();
    try
    {
        await dbInitializer.InitializeDatabaseAsync();
    }
    catch (Exception ex)
    {
        // Ignorar erros de inicialização
    }
}

app.Run();
