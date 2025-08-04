using Microsoft.AspNetCore.Mvc;

namespace FiapProjetoGames.API.Controllers;

[ApiController]
[Route("debug")]
public class DebugController : ControllerBase
{
    [HttpGet("info")]
    public IActionResult Info()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetName().Name)
            .OrderBy(n => n)
            .ToArray();
        
        return Ok(new { 
            Assemblies = assemblies,
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Not set",
            Port = Environment.GetEnvironmentVariable("PORT") ?? "Not set",
            Urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "Not set"
        });
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("Debug controller funcionando!");
    }
} 