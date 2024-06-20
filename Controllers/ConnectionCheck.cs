using Microsoft.AspNetCore.Mvc;

namespace BATTARI_api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ConnectionCheck : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("Connection is working");
    }
}