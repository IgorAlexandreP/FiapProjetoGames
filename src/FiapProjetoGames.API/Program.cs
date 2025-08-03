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
using FiapProjetoGames.API.Middleware;
using FiapProjetoGames.API.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/api-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Iniciando aplicação FIAP Projeto Games API");

    // Add services to the container.
    builder.Services.AddControllers(options =>
    {
        options.SuppressAsyncSuffixInActionNames = false;
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { 
            Title = "FIAP Projeto Games API", 
            Version = "v1.0",
            Description = "API para plataforma educacional de jogos da FIAP"
        });

        // Configure JWT Authentication in Swagger
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
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<ApplicationDbContext>("Database")
        .AddCheck("Self", () => HealthCheckResult.Healthy(), tags: new[] { "self" });

    // Configure CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });

        options.AddPolicy("Production", policy =>
        {
            policy.WithOrigins(
                    "https://fiap-projeto-games.railway.app",
                    "https://*.railway.app",
                    "https://localhost:5001",
                    "http://localhost:5000"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // Configure DbContext
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
    
    // Se a connection string contém variáveis não interpoladas, construir manualmente
    if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("${"))
    {
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "railway";
        var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
        dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
        
        if (!string.IsNullOrEmpty(dbHost) && !string.IsNullOrEmpty(dbPassword))
        {
            connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User Id={dbUser};Password={dbPassword};";
        }
        else
        {
            throw new InvalidOperationException("Environment variables not configured properly.");
        }
    }
    else if (string.IsNullOrEmpty(connectionString))
    {
        // Fallback para variáveis de ambiente individuais
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "railway";
        var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
        dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
        
        if (!string.IsNullOrEmpty(dbHost) && !string.IsNullOrEmpty(dbPassword))
        {
            connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User Id={dbUser};Password={dbPassword};";
        }
        else
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found and environment variables not configured.");
        }
    }
    
    Log.Information("Using connection string: {ConnectionString}", connectionString.Replace(dbPassword ?? "", "***"));
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(connectionString, 
            ServerVersion.AutoDetect(connectionString),
            mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null)));

    // Configure JWT Authentication
    var jwtSecret = builder.Configuration["JwtSettings:Secret"] ?? 
                   Environment.GetEnvironmentVariable("JWT_SECRET") ??
                   throw new InvalidOperationException("JWT Secret not configured");
    var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "FiapProjetoGames";
    var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "FiapProjetoGamesUsers";
    var jwtExpirationHours = int.Parse(builder.Configuration["JwtSettings:ExpirationHours"] ?? "24");
    
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
            ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
            ValidIssuer = jwtIssuer,
            ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
            ValidAudience = jwtAudience,
            ClockSkew = TimeSpan.Zero
        };
    });

    // Configure Authorization
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy =>
            policy.RequireRole("Admin"));
        
        options.AddPolicy("UserOrAdmin", policy =>
            policy.RequireRole("User", "Admin"));
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
        return new UsuarioService(usuarioRepository, logService, jwtSecret, jwtIssuer, jwtAudience, jwtExpirationHours);
    });
    builder.Services.AddScoped<IJogoService, JogoService>();
    builder.Services.AddScoped<IBibliotecaJogoService, BibliotecaJogoService>();

    // Register Validators
    builder.Services.AddScoped<IValidator<CadastroUsuarioDto>, CadastroUsuarioValidation>();
    builder.Services.AddScoped<IValidator<LoginUsuarioDto>, LoginUsuarioValidation>();

    // Configure Memory Cache
    builder.Services.AddMemoryCache();

    // Configure Response Compression
    builder.Services.AddResponseCompression();

    // Register Database Initialization Service
    builder.Services.AddScoped<DatabaseInitializationService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "FIAP Projeto Games API v1.0");
            c.RoutePrefix = "swagger";
            c.DocumentTitle = "FIAP Projeto Games API Documentation";
        });
    }
    else
    {
        // In production, only expose Swagger at a specific path
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "FIAP Projeto Games API v1.0");
            c.RoutePrefix = "api-docs";
        });
    }

    // Configure CORS based on environment
    if (app.Environment.IsDevelopment())
    {
        app.UseCors("AllowAll");
    }
    else
    {
        app.UseCors("Production");
    }

    // Use Response Compression
    app.UseResponseCompression();

    // Global Exception Handler
    app.UseMiddleware<ErrorHandlingMiddleware>();

    // Audit Logging
    app.UseMiddleware<AuditMiddleware>();

    // Rate Limiting
    app.UseMiddleware<RateLimitingMiddleware>();

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Health Checks - Removendo mapeamento conflitante para permitir que o HealthController funcione
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false
    });

    // Map Controllers
    app.MapControllers();

    // Initialize database
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializationService>();
        try
        {
            await dbInitializer.InitializeDatabaseAsync();
            Log.Information("Database initialized successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while initializing the database");
        }
    }

    Log.Information("Aplicação iniciada com sucesso");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação falhou ao iniciar");
}
finally
{
    Log.CloseAndFlush();
}
