using System;
using System.Threading.Tasks;

namespace FiapProjetoGames.Application.Services
{
    public interface ILogService
    {
        Task LogInfoAsync(string message, object? data = null);
        Task LogWarningAsync(string message, object? data = null);
        Task LogErrorAsync(string message, Exception? exception = null, object? data = null);
        Task LogSecurityAsync(string message, object? data = null);
        Task LogAuditAsync(string action, string userId, object? data = null);
    }
} 