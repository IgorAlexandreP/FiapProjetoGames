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
    }
} 