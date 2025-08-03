using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using FiapProjetoGames.Application.Services;

namespace FiapProjetoGames.API.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;

        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;
            var originalBodyStream = context.Response.Body;

            try
            {
                await _next(context);
            }
            finally
            {
                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;

                // Registra métricas para todas as requisições
                var endpoint = context.Request.Path;
                var method = context.Request.Method;
                var statusCode = context.Response.StatusCode;

                // Usar IServiceProvider para obter o serviço scoped
                var metricsService = context.RequestServices.GetService<IMetricsService>();
                if (metricsService != null)
                {
                    await metricsService.IncrementCounterAsync("api_requests", $"{method}_{endpoint}");
                    await metricsService.RecordTimingAsync("api_response_time", duration, $"{method}_{endpoint}");

                    if (statusCode >= 400)
                    {
                        await metricsService.IncrementCounterAsync("api_errors", $"{statusCode}_{method}_{endpoint}");
                    }
                }

                // Log apenas para operações que modificam dados
                if (ShouldAudit(context.Request))
                {
                    var userId = GetUserId(context);
                    var action = $"{context.Request.Method} {context.Request.Path}";

                    _logger.LogInformation(
                        "[AUDIT] User: {UserId}, Action: {Action}, Status: {StatusCode}, Duration: {Duration}ms",
                        userId, action, statusCode, duration.TotalMilliseconds);
                }
            }
        }

        private bool ShouldAudit(HttpRequest request)
        {
            var method = request.Method.ToUpper();
            return method == "POST" || method == "PUT" || method == "DELETE" || method == "PATCH";
        }

        private string GetUserId(HttpContext context)
        {
            var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim?.Value ?? "Anonymous";
        }
    }
} 