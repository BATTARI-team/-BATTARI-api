using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BATTARI_api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class DeveloperController : ControllerBase {
  /// <summary>
  /// ログインしてないと使えません
  /// </summary>
  /// <returns></returns> <summary>
  ///
  /// </summary>
  /// <returns></returns>
  [HttpGet]
  [Authorize]
  public IActionResult ConnectionCheck() {
    var identity = HttpContext.User.Identity as ClaimsIdentity;
    var claim = identity?.Claims.FirstOrDefault(c => c.Type == "name");

    if (claim != null) {
      Console.WriteLine(claim.Value);
    }

    return Ok("Connection is working. Welcome " + claim.Value + "!");
  }

  /// <summary>
  ///
  /// </summary>
  /// <returns></returns>
  [HttpPut]
  public IActionResult JWTParse(String token) {
    var jsonToken = new JwtSecurityTokenHandler().ReadToken(token);
    return Ok(jsonToken);
  }
  [HttpGet]
  public void setSouguu() { WebSocketController.isSouguu = true; }

  [HttpGet]
  public void unsetSouguu() { WebSocketController.isSouguu = false; }
  [HttpGet]
  public bool isSouguu() { return WebSocketController.isSouguu; }
}
