using Microsoft.AspNetCore.Mvc;

namespace FiapProjetoGames.API.Controllers
{
    [ApiController]
    [Route("minimal")]
    public class MinimalController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API funcionando!");
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        [HttpGet("json")]
        public IActionResult Json()
        {
            return Ok(new
            {
                message = "API funcionando!",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }
    }
} 