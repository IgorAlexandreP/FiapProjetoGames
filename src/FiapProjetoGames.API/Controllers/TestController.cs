using Microsoft.AspNetCore.Mvc;

namespace FiapProjetoGames.API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "Test endpoint working!",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }
    }
} 