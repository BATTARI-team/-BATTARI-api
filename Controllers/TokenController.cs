using BATTARI_api.Models;
using BATTARI_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BATTARI_api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class TokenController(IConfiguration configuration) : ControllerBase
{
    /// <summary>
    /// UserIdとPasswordがあっていれば，Tokenを返します
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Login([FromBody] UserLoginModel model)
    {
        if (model is { UserId: "test", Password: "test" })
        {
            Console.WriteLine("login" + configuration["Jwt:Key"] + configuration["Jwt:Issuer"] +
                              configuration["Jwt:Audience"] + model.UserId);
            var token = TokenService.GenerateToken(
                configuration["Jwt:Key"] ?? "",
                configuration["Jwt:Issuer"] ?? "",
                configuration["Jwt:Audience"] ?? "",
                model.UserId
                );
            return Ok(token);
        }

        return Unauthorized();
    }
    
}