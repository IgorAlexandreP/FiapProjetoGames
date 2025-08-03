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
using System.Security.Claims;
using FluentValidation;
using FiapProjetoGames.Application.Validation;
using FiapProjetoGames.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "FIAP Projeto Games API", 
        Version = "v1.0"
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
        ValidateAudience = false
    };
});

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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FIAP Projeto Games API v1.0");
    c.RoutePrefix = "swagger";
});

app.UseCors(builder => builder
       .AllowAnyOrigin()
       .AllowAnyMethod()
       .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
