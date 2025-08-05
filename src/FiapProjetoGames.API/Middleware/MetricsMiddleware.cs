using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using FiapProjetoGames.Application.Services;

namespace FiapProjetoGames.API.Middleware
{
    public class MetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMetricsService _metricsService;

        public MetricsMiddleware(RequestDelegate next, IMetricsService metricsService)
        {
            _next = next;
            _metricsService = metricsService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var path = context.Request.Path.Value ?? "";
            var method = context.Request.Method;
            var statusCode = 200; // Default

            try
            {
                // Sanitizar o path para evitar caracteres especiais
                var sanitizedPath = path.Replace("/", "_").Replace("-", "_");
                
                // Incrementar contador de requisições HTTP
                await _metricsService.IncrementCounterAsync("http_requests_total", $"method={method},endpoint={sanitizedPath},status=200");

                await _next(context);

                statusCode = context.Response.StatusCode;
                
                // Atualizar contador com status code real
                await _metricsService.IncrementCounterAsync("http_requests_total", $"method={method},endpoint={sanitizedPath},status={statusCode}");
            }
            catch (Exception)
            {
                statusCode = 500;
                var sanitizedPath = path.Replace("/", "_").Replace("-", "_");
                await _metricsService.IncrementCounterAsync("http_requests_total", $"method={method},endpoint={sanitizedPath},status={statusCode}");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                
                // Sanitizar o path para evitar caracteres especiais
                var sanitizedPath = path.Replace("/", "_").Replace("-", "_");
                
                // Registrar duração da requisição
                await _metricsService.RecordHistogramAsync("http_request_duration_seconds", stopwatch.Elapsed.TotalSeconds, $"method={method},endpoint={sanitizedPath}");
                
                // Registrar métricas específicas da aplicação
                if (path.StartsWith("/api/"))
                {
                    await _metricsService.IncrementCounterAsync("fiap_games_api_requests_total", $"method={method},endpoint={sanitizedPath}");
                }
                
                // Registrar taxa de erros
                if (statusCode >= 400)
                {
                    await _metricsService.IncrementCounterAsync("fiap_games_error_rate", $"status={statusCode},endpoint={sanitizedPath}");
                }
            }
        }
    }
} 