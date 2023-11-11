using Microsoft.AspNetCore.Mvc;

namespace Rsoi.Lab3.RatingService.HttpApi.Controllers;

public class HealthController : ControllerBase
{
    [HttpGet]
    [Route("manage/health")]
    public IActionResult HealthCheck()
    {
        return Ok();
    }
}