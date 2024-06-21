using System.IdentityModel.Tokens.Jwt;
using BATTARI_api.Models;
using BATTARI_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BATTARI_api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class TokenController(IConfiguration configuration) : ControllerBase
{
    [HttpPost("Login")]
    public IActionResult Login([FromBody] UserLoginModel model)
    {
        if (model.UserId == "test" && model.Password == "test")
        {
            Console.WriteLine("login" + configuration["Jwt:Key"] + configuration["Jwt:Issuer"] +
                              configuration["Jwt:Audience"] + model.UserId);
            var token = TokenService.GenerateToken(
                configuration["Jwt:Key"],
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                model.UserId
                );
            return Ok(token);
        }

        return Unauthorized();
    }
    
}