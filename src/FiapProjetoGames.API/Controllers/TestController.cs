using Microsoft.AspNetCore.Mvc;

namespace FiapProjetoGames.API.Controllers
{
    [ApiController]
    [Route("")]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// Endpoint de teste na raiz
        /// </summary>
        /// <returns>Teste simples</returns>
        [HttpGet]
        public IActionResult Get()
        {
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

        /// <summary>
        /// Endpoint de teste simples
        /// </summary>
        /// <returns>Teste simples</returns>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("API funcionando!");
        }

        /// <summary>
        /// Endpoint de ping
        /// </summary>
        /// <returns>Pong</returns>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        /// <summary>
        /// Endpoint de debug para verificar roteamento
        /// </summary>
        /// <returns>Informações de debug</returns>
        [HttpGet("debug")]
        public IActionResult Debug()
        {
            return Ok(new
            {
                message = "Debug endpoint funcionando!",
                timestamp = DateTime.UtcNow,
                request = new
                {
                    method = Request.Method,
                    path = Request.Path,
                    query = Request.QueryString.ToString(),
                    headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                    host = Request.Host.ToString(),
                    scheme = Request.Scheme,
                    protocol = Request.Protocol
                },
                environment = new
                {
                    aspnetcore_environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                    aspnetcore_urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS"),
                    port = Environment.GetEnvironmentVariable("PORT")
                }
            });
        }
    }
} 