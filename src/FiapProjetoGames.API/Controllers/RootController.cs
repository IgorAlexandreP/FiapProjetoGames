using Microsoft.AspNetCore.Mvc;

namespace FiapProjetoGames.API.Controllers
{
    [ApiController]
    [Route("")]
    public class RootController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "FIAP Projeto Games API funcionando!",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                database = "In-Memory",
                version = "1.0.0",
                endpoints = new[]
                {
                    "/health",
                    "/debug",
                    "/api/usuarios/cadastro",
                    "/api/usuarios/login",
                    "/api/jogos",
                    "/api/biblioteca"
                }
            });
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok(new
            {
                status = "running",
                timestamp = DateTime.UtcNow,
                uptime = Environment.TickCount64
            });
        }
    }
} 