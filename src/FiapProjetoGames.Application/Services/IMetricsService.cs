using System;
using System.Threading.Tasks;

namespace FiapProjetoGames.Application.Services
{
    public interface IMetricsService
    {
        Task IncrementCounterAsync(string metricName, string? label = null);
        Task RecordHistogramAsync(string metricName, double value, string? label = null);
        Task RecordGaugeAsync(string metricName, double value, string? label = null);
        Task RecordTimingAsync(string metricName, TimeSpan duration, string? label = null);
        Task<string> GetMetricsAsync();
    }
} 