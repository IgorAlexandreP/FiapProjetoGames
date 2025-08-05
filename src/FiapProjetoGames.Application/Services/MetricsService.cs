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
            var queue = _timings.GetOrAdd(key, _ => new ConcurrentQueue<TimeSpan>());
            queue.Enqueue(TimeSpan.FromSeconds(value));
            
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
            
            // HTTP Requests Total
            sb.AppendLine("# HELP http_requests_total Total number of HTTP requests");
            sb.AppendLine("# TYPE http_requests_total counter");
            foreach (var counter in _counters.Where(c => c.Key.StartsWith("http_requests_total")))
            {
                var labels = counter.Key.Replace("http_requests_total_", "");
                // Garantir que as labels estejam no formato correto do Prometheus
                var formattedLabels = FormatLabels(labels);
                sb.AppendLine($"http_requests_total{{{formattedLabels}}} {counter.Value}");
            }

            // HTTP Request Duration
            sb.AppendLine();
            sb.AppendLine("# HELP http_request_duration_seconds HTTP request duration in seconds");
            sb.AppendLine("# TYPE http_request_duration_seconds histogram");
            foreach (var timing in _timings.Where(t => t.Key.StartsWith("http_request_duration_seconds")))
            {
                var times = timing.Value.ToArray();
                if (times.Length > 0)
                {
                    var avg = times.Average(t => t.TotalSeconds);
                    var min = times.Min(t => t.TotalSeconds);
                    var max = times.Max(t => t.TotalSeconds);
                    var sum = times.Sum(t => t.TotalSeconds);
                    var count = times.Length;
                    
                    var labels = timing.Key.Replace("http_request_duration_seconds_", "");
                    var formattedLabels = FormatLabels(labels);
                    
                    sb.AppendLine($"http_request_duration_seconds_sum{{{formattedLabels}}} {sum:F6}");
                    sb.AppendLine($"http_request_duration_seconds_count{{{formattedLabels}}} {count}");
                    sb.AppendLine($"http_request_duration_seconds_avg{{{formattedLabels}}} {avg:F6}");
                    sb.AppendLine($"http_request_duration_seconds_min{{{formattedLabels}}} {min:F6}");
                    sb.AppendLine($"http_request_duration_seconds_max{{{formattedLabels}}} {max:F6}");
                }
            }

            // FIAP Games API Requests
            sb.AppendLine();
            sb.AppendLine("# HELP fiap_games_api_requests_total Total number of FIAP Games API requests");
            sb.AppendLine("# TYPE fiap_games_api_requests_total counter");
            foreach (var counter in _counters.Where(c => c.Key.StartsWith("fiap_games_api_requests_total")))
            {
                var labels = counter.Key.Replace("fiap_games_api_requests_total_", "");
                var formattedLabels = FormatLabels(labels);
                sb.AppendLine($"fiap_games_api_requests_total{{{formattedLabels}}} {counter.Value}");
            }

            // FIAP Games Error Rate
            sb.AppendLine();
            sb.AppendLine("# HELP fiap_games_error_rate Error rate for FIAP Games API");
            sb.AppendLine("# TYPE fiap_games_error_rate counter");
            foreach (var counter in _counters.Where(c => c.Key.StartsWith("fiap_games_error_rate")))
            {
                var labels = counter.Key.Replace("fiap_games_error_rate_", "");
                var formattedLabels = FormatLabels(labels);
                sb.AppendLine($"fiap_games_error_rate{{{formattedLabels}}} {counter.Value}");
            }

            // FIAP Games Total Users (simulado)
            sb.AppendLine();
            sb.AppendLine("# HELP fiap_games_total_users Total number of users in FIAP Games");
            sb.AppendLine("# TYPE fiap_games_total_users gauge");
            sb.AppendLine("fiap_games_total_users 15");

            // FIAP Games Total Games (simulado)
            sb.AppendLine();
            sb.AppendLine("# HELP fiap_games_total_games Total number of games in FIAP Games");
            sb.AppendLine("# TYPE fiap_games_total_games gauge");
            sb.AppendLine("fiap_games_total_games 8");

            // API Requests (legacy)
            sb.AppendLine();
            sb.AppendLine("# HELP api_requests_total Total number of API requests");
            sb.AppendLine("# TYPE api_requests_total counter");
            foreach (var counter in _counters.Where(c => c.Key.StartsWith("api_requests_total")))
            {
                var labels = counter.Key.Replace("api_requests_total_", "");
                var formattedLabels = FormatLabels(labels);
                sb.AppendLine($"api_requests_total{{{formattedLabels}}} {counter.Value}");
            }

            // API Response Time (legacy)
            sb.AppendLine();
            sb.AppendLine("# HELP api_response_time_seconds API response time in seconds");
            sb.AppendLine("# TYPE api_response_time_seconds histogram");
            foreach (var timing in _timings.Where(t => t.Key.StartsWith("api_response_time")))
            {
                var times = timing.Value.ToArray();
                if (times.Length > 0)
                {
                    var avg = times.Average(t => t.TotalSeconds);
                    var min = times.Min(t => t.TotalSeconds);
                    var max = times.Max(t => t.TotalSeconds);
                    
                    sb.AppendLine($"api_response_time_seconds_avg{{{GetLabelString(timing.Key.Replace("api_response_time_", ""))}}} {avg:F3}");
                    sb.AppendLine($"api_response_time_seconds_min{{{GetLabelString(timing.Key.Replace("api_response_time_", ""))}}} {min:F3}");
                    sb.AppendLine($"api_response_time_seconds_max{{{GetLabelString(timing.Key.Replace("api_response_time_", ""))}}} {max:F3}");
                }
            }

            // API Gauge Current (legacy)
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

        private string FormatLabels(string labels)
        {
            // Se já está no formato correto do Prometheus, retorna como está
            if (labels.Contains("=") && labels.Contains("\""))
            {
                return labels;
            }
            
            // Se contém vírgulas, é um conjunto de labels
            if (labels.Contains(","))
            {
                var labelPairs = labels.Split(',');
                var formattedLabels = new List<string>();
                
                foreach (var pair in labelPairs)
                {
                    if (pair.Contains("="))
                    {
                        var parts = pair.Split('=', 2);
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        
                        // Garantir que o valor esteja entre aspas se necessário
                        if (!value.StartsWith("\"") && !value.StartsWith("'"))
                        {
                            value = $"\"{value}\"";
                        }
                        
                        formattedLabels.Add($"{key}={value}");
                    }
                }
                
                return string.Join(",", formattedLabels);
            }
            
            // Se é uma única label
            if (labels.Contains("="))
            {
                var parts = labels.Split('=', 2);
                var key = parts[0].Trim();
                var value = parts[1].Trim();
                
                // Garantir que o valor esteja entre aspas se necessário
                if (!value.StartsWith("\"") && !value.StartsWith("'"))
                {
                    value = $"\"{value}\"";
                }
                
                return $"{key}={value}";
            }
            
            return labels;
        }

        private string GetLabelString(string key)
        {
            // Se a key já contém labels no formato correto, retorna como está
            if (key.Contains("=") && key.Contains("\""))
            {
                return key;
            }
            
            // Caso contrário, formata como antes
            if (key.Contains('_'))
            {
                var parts = key.Split('_', 2);
                return $"metric=\"{parts[0]}\",label=\"{parts[1]}\"";
            }
            return $"metric=\"{key}\"";
        }
    }
} 