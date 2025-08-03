using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FiapProjetoGames.Application.Services
{
    public class MetricsService : IMetricsService
    {
        private readonly ILogger<MetricsService> _logger;
        private readonly ConcurrentDictionary<string, long> _counters;
        private readonly ConcurrentDictionary<string, ConcurrentQueue<double>> _histograms;
        private readonly ConcurrentDictionary<string, double> _gauges;
        private readonly ConcurrentDictionary<string, ConcurrentQueue<TimeSpan>> _timings;

        public MetricsService(ILogger<MetricsService> logger)
        {
            _logger = logger;
            _counters = new ConcurrentDictionary<string, long>();
            _histograms = new ConcurrentDictionary<string, ConcurrentQueue<double>>();
            _gauges = new ConcurrentDictionary<string, double>();
            _timings = new ConcurrentDictionary<string, ConcurrentQueue<TimeSpan>>();
        }

        public async Task IncrementCounterAsync(string metricName, string? label = null)
        {
            var key = GetMetricKey(metricName, label);
            _counters.AddOrUpdate(key, 1, (k, v) => v + 1);
            await Task.CompletedTask;
        }

        public async Task RecordHistogramAsync(string metricName, double value, string? label = null)
        {
            var key = GetMetricKey(metricName, label);
            var queue = _histograms.GetOrAdd(key, _ => new ConcurrentQueue<double>());
            queue.Enqueue(value);
            
            // Manter apenas os últimos 1000 valores
            while (queue.Count > 1000)
            {
                queue.TryDequeue(out _);
            }
            
            await Task.CompletedTask;
        }

        public async Task RecordGaugeAsync(string metricName, double value, string? label = null)
        {
            var key = GetMetricKey(metricName, label);
            _gauges.AddOrUpdate(key, value, (k, v) => value);
            await Task.CompletedTask;
        }

        public async Task RecordTimingAsync(string metricName, TimeSpan duration, string? label = null)
        {
            var key = GetMetricKey(metricName, label);
            var queue = _timings.GetOrAdd(key, _ => new ConcurrentQueue<TimeSpan>());
            queue.Enqueue(duration);
            
            // Manter apenas os últimos 1000 valores
            while (queue.Count > 1000)
            {
                queue.TryDequeue(out _);
            }
            
            await Task.CompletedTask;
        }

        public async Task<string> GetMetricsAsync()
        {
            var sb = new StringBuilder();
            sb.AppendLine("# HELP api_requests_total Total number of API requests");
            sb.AppendLine("# TYPE api_requests_total counter");

            foreach (var counter in _counters)
            {
                sb.AppendLine($"api_requests_total{{{GetLabelString(counter.Key)}}} {counter.Value}");
            }

            sb.AppendLine();
            sb.AppendLine("# HELP api_response_time_seconds API response time in seconds");
            sb.AppendLine("# TYPE api_response_time_seconds histogram");

            foreach (var timing in _timings)
            {
                var times = timing.Value.ToArray();
                if (times.Length > 0)
                {
                    var avg = times.Average(t => t.TotalSeconds);
                    var min = times.Min(t => t.TotalSeconds);
                    var max = times.Max(t => t.TotalSeconds);
                    
                    sb.AppendLine($"api_response_time_seconds_avg{{{GetLabelString(timing.Key)}}} {avg:F3}");
                    sb.AppendLine($"api_response_time_seconds_min{{{GetLabelString(timing.Key)}}} {min:F3}");
                    sb.AppendLine($"api_response_time_seconds_max{{{GetLabelString(timing.Key)}}} {max:F3}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("# HELP api_gauge_current Current gauge values");
            sb.AppendLine("# TYPE api_gauge_current gauge");

            foreach (var gauge in _gauges)
            {
                sb.AppendLine($"api_gauge_current{{{GetLabelString(gauge.Key)}}} {gauge.Value:F3}");
            }

            return await Task.FromResult(sb.ToString());
        }

        private string GetMetricKey(string metricName, string? label)
        {
            return string.IsNullOrEmpty(label) ? metricName : $"{metricName}_{label}";
        }

        private string GetLabelString(string key)
        {
            if (key.Contains('_'))
            {
                var parts = key.Split('_', 2);
                return $"metric=\"{parts[0]}\",label=\"{parts[1]}\"";
            }
            return $"metric=\"{key}\"";
        }
    }
} 