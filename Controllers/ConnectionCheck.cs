using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BATTARI_api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class DeveloperController : ControllerBase
{
    /// <summary>
    /// ログインしてないと使えません
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public IActionResult ConnectionCheck()
    {
        return Ok("Connection is working");
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    public IActionResult JWTParse(String token)
    {
        var jsonToken = new JwtSecurityTokenHandler().ReadToken(token);
        return Ok(jsonToken);
    }
}