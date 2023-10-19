using Microsoft.AspNetCore.Mvc;

namespace JhipsterDotNetMS.Controllers;

[ApiController]
[Route("api")]
public class HealthCheckController : ControllerBase
{
    
    [HttpGet]
    [Route("health")]
    public IActionResult GetHealth()
    {
        var myObject = new {
            Status = "UP"
        };
        return Ok(myObject);
    }
}
