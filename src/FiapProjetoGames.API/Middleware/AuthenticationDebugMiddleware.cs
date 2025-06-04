using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Security.Claims;

namespace FiapProjetoGames.API.Middleware
{
    public class AuthenticationDebugMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationDebugMiddleware> _logger;

        public AuthenticationDebugMiddleware(RequestDelegate next, ILogger<AuthenticationDebugMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Request Path: {Path}", context.Request.Path);
            _logger.LogInformation("Request Method: {Method}", context.Request.Method);

            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);

                    _logger.LogInformation("Token JWT válido:");
                    _logger.LogInformation("Emissor: {Issuer}", jwtToken.Issuer);
                    _logger.LogInformation("Audiência: {Audience}", jwtToken.Audiences?.FirstOrDefault());
                    _logger.LogInformation("Válido a partir de: {NotBefore}", jwtToken.ValidFrom);
                    _logger.LogInformation("Expira em: {Expires}", jwtToken.ValidTo);
                    _logger.LogInformation("Claims:");
                    foreach (var claim in jwtToken.Claims)
                    {
                        _logger.LogInformation("  {Type}: {Value}", claim.Type, claim.Value);
                    }

                    await _next(context);

                    // Log após a execução do pipeline
                    _logger.LogInformation("Após autenticação - Usuário autenticado: {IsAuthenticated}", context.User?.Identity?.IsAuthenticated);
                    if (context.User?.Identity?.IsAuthenticated == true)
                    {
                        _logger.LogInformation("Claims do usuário após autenticação:");
                        foreach (var claim in context.User.Claims)
                        {
                            _logger.LogInformation("  {Type}: {Value}", claim.Type, claim.Value);
                        }
                        _logger.LogInformation("Roles do usuário:");
                        foreach (var role in context.User.Claims.Where(c => c.Type == "role" || c.Type == ClaimTypes.Role))
                        {
                            _logger.LogInformation("  Role: {Role}", role.Value);
                        }
                        _logger.LogInformation("IsInRole('Admin'): {IsInRole}", context.User.IsInRole("Admin"));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar token JWT");
                    await _next(context);
                }
            }
            else
            {
                _logger.LogWarning("Nenhum token de autorização encontrado");
                await _next(context);
            }
        }
    }
} 