using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FiapProjetoGames.Application.Services;

namespace FiapProjetoGames.API.Controllers
{
    [ApiController]
    [Route("api/metrics")]
    [Authorize(Roles = "Admin")]
    public class MetricsController : ControllerBase
    {
        private readonly IMetricsService _metricsService;

        public MetricsController(IMetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        /// <summary>
        /// Obtém as métricas da aplicação em formato Prometheus
        /// </summary>
        /// <returns>Métricas em formato Prometheus</returns>
        [HttpGet]
        public async Task<IActionResult> GetMetrics()
        {
            var metrics = await _metricsService.GetMetricsAsync();
            return Content(metrics, "text/plain");
        }

        /// <summary>
        /// Incrementa um contador de métrica
        /// </summary>
        /// <param name="metricName">Nome da métrica</param>
        /// <param name="label">Label opcional</param>
        /// <returns>Confirmação da operação</returns>
        [HttpPost("counter/{metricName}")]
        public async Task<IActionResult> IncrementCounter(string metricName, [FromQuery] string? label = null)
        {
            await _metricsService.IncrementCounterAsync(metricName, label);
            return Ok(new { message = "Contador incrementado com sucesso" });
        }

        /// <summary>
        /// Registra um valor de histograma
        /// </summary>
        /// <param name="metricName">Nome da métrica</param>
        /// <param name="value">Valor a ser registrado</param>
        /// <param name="label">Label opcional</param>
        /// <returns>Confirmação da operação</returns>
        [HttpPost("histogram/{metricName}")]
        public async Task<IActionResult> RecordHistogram(string metricName, [FromQuery] double value, [FromQuery] string? label = null)
        {
            await _metricsService.RecordHistogramAsync(metricName, value, label);
            return Ok(new { message = "Histograma registrado com sucesso" });
        }

        /// <summary>
        /// Registra um valor de gauge
        /// </summary>
        /// <param name="metricName">Nome da métrica</param>
        /// <param name="value">Valor a ser registrado</param>
        /// <param name="label">Label opcional</param>
        /// <returns>Confirmação da operação</returns>
        [HttpPost("gauge/{metricName}")]
        public async Task<IActionResult> RecordGauge(string metricName, [FromQuery] double value, [FromQuery] string? label = null)
        {
            await _metricsService.RecordGaugeAsync(metricName, value, label);
            return Ok(new { message = "Gauge registrado com sucesso" });
        }
    }
} 