using Microsoft.AspNetCore.Mvc;

namespace FiapProjetoGames.API.Controllers
{
    [ApiController]
    [Route("api/monitoring")]
    public class MonitoringController : ControllerBase
    {
        /// <summary>
        /// Informações sobre o sistema de monitoramento
        /// </summary>
        /// <returns>Links e instruções de monitoramento</returns>
        [HttpGet]
        public IActionResult GetMonitoringInfo()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var monitoringInfo = new
            {
                message = "Sistema de monitoramento FCG - FIAP Challenge Games",
                status = "online",
                endpoints = new
                {
                    health = $"{baseUrl}/health",
                    metrics = $"{baseUrl}/api/metrics (requer autenticação Admin)",
                    swagger = $"{baseUrl}/swagger"
                },
                local_monitoring = new
                {
                    grafana = "http://localhost:3000 (admin/admin)",
                    prometheus = "http://localhost:9090",
                    setup_command = "docker-compose up -d"
                },
                railway_note = "No Railway, apenas as métricas da API estão disponíveis. Para monitoramento completo, execute localmente."
            };
            
            return Ok(monitoringInfo);
        }

        /// <summary>
        /// Status simplificado do sistema
        /// </summary>
        /// <returns>Status básico</returns>
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                status = "online",
                timestamp = DateTime.UtcNow,
                service = "FCG API",
                version = "1.0.0"
            });
        }
    }
} 