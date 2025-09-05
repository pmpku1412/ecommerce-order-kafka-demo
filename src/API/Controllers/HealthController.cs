using Microsoft.AspNetCore.Mvc;
using API.Controllers;

namespace TQM.BackOffice.API.Controllers;

[Route("[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> GetHealth()
    {
        var healthStatus = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
            Services = new
            {
                Database = "Connected", // ในการใช้งานจริงจะตรวจสอบ database connection
                Kafka = "Connected",    // ในการใช้งานจริงจะตรวจสอบ Kafka connection
                API = "Running"
            }
        };

        return Ok(healthStatus);
    }

    [HttpGet("ping")]
    [AllowAnonymous]
    public ActionResult Ping()
    {
        return Ok(new { Message = "Pong", Timestamp = DateTime.UtcNow });
    }

    [HttpGet("version")]
    [AllowAnonymous]
    public ActionResult GetVersion()
    {
        var version = new
        {
            Version = "1.0.0",
            BuildDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            Framework = ".NET 6",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"
        };

        return Ok(version);
    }
}
