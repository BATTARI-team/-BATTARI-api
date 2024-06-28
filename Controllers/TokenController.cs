using BATTARI_api.Models;
using BATTARI_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BATTARI_api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly ITokenService tokenService;
    private readonly IConfiguration configuration;
    public TokenController(ITokenService tokenService, IConfiguration configuration)
    {
        this.tokenService = tokenService;
        this.configuration = configuration;
    }
    /// <summary>
    /// UserIdとPasswordがあっていれば，Tokenを返します
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Login([FromBody] UserLoginModel model)
    {
        
        // if (model is { UserId: "test", Password: "test" })
        // {
        //     Console.WriteLine("login" + configuration["Jwt:Key"] + configuration["Jwt:Issuer"] +
        //                       configuration["Jwt:Audience"] + model.UserId);
        //     var token = tokenService.GenerateToken(
        //         configuration["Jwt:Key"] ?? "",
        //         model
        //         );
        //     return Ok(token);
        // }

        return Unauthorized();
    }
    
}