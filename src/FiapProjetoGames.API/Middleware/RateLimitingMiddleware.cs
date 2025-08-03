using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FiapProjetoGames.API.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimitStore = new();

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = GetClientId(context);
            var endpoint = context.Request.Path;

            if (IsRateLimited(clientId, endpoint))
            {
                _logger.LogWarning("[RATE_LIMIT] Client {ClientId} exceeded rate limit for {Endpoint}", clientId, endpoint);
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            await _next(context);
        }

        private string GetClientId(HttpContext context)
        {
            // Usar IP do cliente como identificador
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private bool IsRateLimited(string clientId, string endpoint)
        {
            var key = $"{clientId}:{endpoint}";
            var now = DateTime.UtcNow;

            if (_rateLimitStore.TryGetValue(key, out var info))
            {
                // Limpar registros antigos (mais de 1 minuto)
                if (now - info.FirstRequest > TimeSpan.FromMinutes(1))
                {
                    _rateLimitStore.TryRemove(key, out _);
                    info = new RateLimitInfo();
                }
            }
            else
            {
                info = new RateLimitInfo();
            }

            info.RequestCount++;
            info.LastRequest = now;

            if (info.RequestCount == 1)
            {
                info.FirstRequest = now;
            }

            _rateLimitStore.AddOrUpdate(key, info, (k, v) => info);

            // Limite: 100 requests por minuto por cliente/endpoint
            return info.RequestCount > 100;
        }

        private class RateLimitInfo
        {
            public int RequestCount { get; set; }
            public DateTime FirstRequest { get; set; } = DateTime.UtcNow;
            public DateTime LastRequest { get; set; } = DateTime.UtcNow;
        }
    }
} 