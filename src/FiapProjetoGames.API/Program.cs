using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FiapProjetoGames.Application.Services;
using FiapProjetoGames.Domain.Repositories;
using FiapProjetoGames.Infrastructure.Data;
using FiapProjetoGames.Infrastructure.Repositories;
using FiapProjetoGames.API.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using FluentValidation;
using FiapProjetoGames.Application.Validation;
using FiapProjetoGames.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                );

            return new BadRequestObjectResult(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title = "Erro de Validação",
                status = 400,
                errors = errors
            });
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "FIAP Projeto Games API - Fase 2", 
        Version = "v2.0",
        Description = "API para gerenciamento de jogos digitais da FIAP - Segunda Fase com melhorias de segurança, logs e performance"
    });
    
    c.EnableAnnotations();

    // Configuração do esquema de segurança JWT
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
            Array.Empty<string>()
        }
    });
});

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

// Configure JWT Authentication
var jwtSecret = builder.Configuration["JwtSettings:Secret"] ?? 
    throw new InvalidOperationException("JWT Secret not configured");
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
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

// Adiciona configuração de autorização com comparação case-insensitive
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy
        .RequireRole("Admin", "ADMIN", "admin"));
});

// Configure Memory Cache
builder.Services.AddMemoryCache();

// Register Services
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
    return new UsuarioService(usuarioRepository, logService, jwtSecret);
});
builder.Services.AddScoped<IJogoService, JogoService>();
builder.Services.AddScoped<IBibliotecaJogoService, BibliotecaJogoService>();

// Register Validators
builder.Services.AddScoped<IValidator<CadastroUsuarioDto>, CadastroUsuarioValidation>();
builder.Services.AddScoped<IValidator<LoginUsuarioDto>, LoginUsuarioValidation>();
builder.Services.AddScoped<IValidator<AtualizacaoUsuarioDto>, AtualizacaoUsuarioValidation>();
builder.Services.AddScoped<IValidator<AtualizacaoSenhaDto>, AtualizacaoSenhaValidation>();
builder.Services.AddScoped<IValidator<UsuarioFiltroDto>, UsuarioFiltroValidation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FIAP Projeto Games API v2.0");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseCors(builder => builder
       .AllowAnyOrigin()
       .AllowAnyMethod()
       .AllowAnyHeader());

app.UseHttpsRedirection();

// Adiciona os middlewares na ordem correta
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<ValidationMiddleware>();
app.UseMiddleware<AuditMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Adiciona o middleware de debug de autenticação apenas em desenvolvimento (após autenticação)
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<AuthenticationDebugMiddleware>();
}

app.MapControllers();

app.Run();
