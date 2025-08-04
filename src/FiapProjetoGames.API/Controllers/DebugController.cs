using Microsoft.AspNetCore.Mvc;

namespace FiapProjetoGames.API.Controllers
{
    [ApiController]
    [Route("debug")]
    public class DebugController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "Debug endpoint funcionando!",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                database = "In-Memory"
            });
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        [HttpGet("info")]
        public IActionResult Info()
        {
            return Ok(new
            {
                message = "API funcionando!",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                database = "In-Memory",
                version = "1.0.0"
            });
        }
    }
} 