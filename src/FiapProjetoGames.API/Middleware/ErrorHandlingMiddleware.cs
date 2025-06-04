using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FiapProjetoGames.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Log de debug para autenticação
                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    _logger.LogInformation("Token recebido: {Token}", context.Request.Headers["Authorization"].ToString());
                    _logger.LogInformation("Path da requisição: {Path}", context.Request.Path);
                    _logger.LogInformation("Método da requisição: {Method}", context.Request.Method);
                    if (context.User?.Identity?.IsAuthenticated == true)
                    {
                        _logger.LogInformation("Usuário autenticado: {UserId}", context.User.FindFirst("nameid")?.Value);
                        _logger.LogInformation("Role do usuário: {Role}", context.User.FindFirst("role")?.Value);
                    }
                    else
                    {
                        _logger.LogWarning("Usuário não autenticado com token presente");
                    }
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Um erro ocorreu durante o processamento da requisição");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new { type = "https://tools.ietf.org/html/rfc7231#section-6.5.1" };
            object errorResponse;

            switch (exception)
            {
                case ValidationException validationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new
                    {
                        type = response.type,
                        title = "Erro de Validação",
                        status = context.Response.StatusCode,
                        errors = new Dictionary<string, string[]>
                        {
                            { "Validação", new[] { validationEx.Message } }
                        }
                    };
                    break;

                case InvalidOperationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new
                    {
                        type = response.type,
                        title = "Erro de Operação",
                        status = context.Response.StatusCode,
                        errors = new Dictionary<string, string[]>
                        {
                            { "Operação", new[] { exception.Message } }
                        }
                    };
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse = new
                    {
                        type = response.type,
                        title = "Acesso Não Autorizado",
                        status = context.Response.StatusCode,
                        errors = new Dictionary<string, string[]>
                        {
                            { "Autorização", new[] { "Você não tem permissão para acessar este recurso" } }
                        }
                    };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new
                    {
                        type = response.type,
                        title = "Erro Interno do Servidor",
                        status = context.Response.StatusCode,
                        errors = new Dictionary<string, string[]>
                        {
                            { "Servidor", new[] { "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde." } }
                        }
                    };
                    break;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
        }
    }
} 