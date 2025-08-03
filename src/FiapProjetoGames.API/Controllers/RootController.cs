using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FiapProjetoGames.API.Controllers
{
    [ApiController]
    [Route("")]
    public class RootController : ControllerBase
    {
        private readonly ILogger<RootController> _logger;

        public RootController(ILogger<RootController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Acesso à raiz da API");
            
            return Ok(new
            {
                message = "FIAP Projeto Games API está funcionando!",
                version = "2.0.0",
                timestamp = DateTime.UtcNow,
                endpoints = new
                {
                    health = "/health",
                    swagger = "/swagger",
                    usuarios = "/api/usuarios",
                    jogos = "/api/jogos",
                    biblioteca = "/api/biblioteca",
                    metrics = "/api/metrics"
                }
            });
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                message = "Teste de conectividade",
                status = "OK",
                timestamp = DateTime.UtcNow
            });
        }
    }
} 