using System.Security.Claims;
using BATTARI_api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class CallController : ControllerBase
{
    private readonly ILogger<CallController> _logger;

    public CallController(ILogger<CallController> logger) { _logger = logger; }

    [HttpGet]
    public String GetCallDetails()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var claim = identity?.Claims.FirstOrDefault(
            c => c.Type == Program.Jwt_Unique_Name_Str);

        if (claim != null)
        {
            Console.WriteLine(claim.Value);
            return claim.Value;
        }
        return "";
    }
}
