// backend/JCertPreBackend/Controllers/HealthController.cs
using Microsoft.AspNetCore.Mvc;

namespace JCertPreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        // GET: api/health
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { 
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            });
        }

        // GET: api/health/cors-test
        [HttpGet("cors-test")]
        public IActionResult CorsTest()
        {
            return Ok(new { 
                cors = "enabled",
                origin = Request.Headers["Origin"].FirstOrDefault() ?? "not-specified",
                method = Request.Method,
                timestamp = DateTime.UtcNow,
                message = "If you can see this, CORS is working!"
            });
        }
    }
}
