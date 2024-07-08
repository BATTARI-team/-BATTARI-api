using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
			var identity = HttpContext.User.Identity as ClaimsIdentity;

			return Ok("Connection is working. Welcome " + identity.Claims.All((claim) => {
						Console.WriteLine(claim.Type + " : " + claim.Value);
						return true;
				}));
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
