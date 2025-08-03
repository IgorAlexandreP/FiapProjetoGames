using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FiapProjetoGames.Application.Services
{
    public class LogService : ILogService
    {
        private readonly ILogger<LogService> _logger;

        public LogService(ILogger<LogService> logger)
        {
            _logger = logger;
        }

        public async Task LogInfoAsync(string message, object? data = null)
        {
            _logger.LogInformation(message, data);
            await Task.CompletedTask;
        }

        public async Task LogWarningAsync(string message, object? data = null)
        {
            _logger.LogWarning(message, data);
            await Task.CompletedTask;
        }

        public async Task LogErrorAsync(string message, Exception? exception = null, object? data = null)
        {
            if (exception != null)
            {
                _logger.LogError(exception, message, data);
            }
            else
            {
                _logger.LogError(message, data);
            }
            await Task.CompletedTask;
        }

        public async Task LogSecurityAsync(string message, object? data = null)
        {
            _logger.LogWarning("[SECURITY] {Message}", message, data);
            await Task.CompletedTask;
        }

        public async Task LogAuditAsync(string action, string userId, object? data = null)
        {
            _logger.LogInformation("[AUDIT] Action: {Action}, User: {UserId}, Data: {@Data}", action, userId, data);
            await Task.CompletedTask;
        }
    }
} 