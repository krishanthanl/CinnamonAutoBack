using Microsoft.AspNetCore.Mvc;

namespace Cinnamon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("ALL OK");
}
