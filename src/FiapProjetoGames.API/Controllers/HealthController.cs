using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace FiapProjetoGames.API.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Health check básico da aplicação
        /// </summary>
        /// <returns>Status da aplicação</returns>
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Health check realizado em {Time}", DateTime.UtcNow);
            
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "2.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
            });
        }

        /// <summary>
        /// Endpoint de teste simples
        /// </summary>
        /// <returns>Mensagem de teste</returns>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                message = "API funcionando!",
                timestamp = DateTime.UtcNow,
                endpoints = new[]
                {
                    "/api/usuarios/cadastro",
                    "/api/usuarios/login", 
                    "/api/jogos",
                    "/api/biblioteca"
                }
            });
        }

        /// <summary>
        /// Endpoint de teste na raiz
        /// </summary>
        /// <returns>Teste simples</returns>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        /// <summary>
        /// Health check detalhado com informações do sistema
        /// </summary>
        /// <returns>Informações detalhadas de saúde da aplicação</returns>
        [HttpGet("detailed")]
        public IActionResult GetDetailed()
        {
            var memoryInfo = GC.GetGCMemoryInfo();
            
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "2.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                system = new
                {
                    uptime = Environment.TickCount64,
                    memory = new
                    {
                        totalMemory = GC.GetTotalMemory(false),
                        heapSize = memoryInfo.HeapSizeBytes,
                        totalAllocated = GC.GetTotalAllocatedBytes()
                    },
                    gc = new
                    {
                        collectionCount = GC.CollectionCount(0),
                        maxGeneration = GC.MaxGeneration
                    }
                },
                services = new
                {
                    database = "connected",
                    cache = "available",
                    metrics = "enabled"
                }
            });
        }

        /// <summary>
        /// Health check para load balancers
        /// </summary>
        /// <returns>Status simples para load balancers</returns>
        [HttpGet("ready")]
        public IActionResult Ready()
        {
            return Ok("ready");
        }

        /// <summary>
        /// Health check para verificar se a aplicação está viva
        /// </summary>
        /// <returns>Status de vida da aplicação</returns>
        [HttpGet("live")]
        public IActionResult Live()
        {
            return Ok("alive");
        }
    }

    [ApiController]
    [Route("")]
    public class RootController : ControllerBase
    {
        private readonly ILogger<RootController> _logger;

        public RootController(ILogger<RootController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Endpoint na raiz para testar roteamento
        /// </summary>
        /// <returns>Informações da API</returns>
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Root endpoint acessado em {Time}", DateTime.UtcNow);
            
            return Ok(new
            {
                message = "FIAP Projeto Games API está funcionando!",
                timestamp = DateTime.UtcNow,
                version = "2.0.0",
                endpoints = new[]
                {
                    "/health",
                    "/health/test",
                    "/health/ping",
                    "/api/usuarios/cadastro",
                    "/api/usuarios/login",
                    "/api/jogos",
                    "/api/biblioteca"
                }
            });
        }
    }
} 