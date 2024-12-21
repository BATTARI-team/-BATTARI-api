using BATTARI_api.Interfaces;
using BATTARI_api.Interfaces.Service;
using BATTARI_api.Models;
using Microsoft.AspNetCore.Mvc;
using webUserLoginTest.Util;

namespace BATTARI_api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class TokenController(ITokenService tokenService, IConfiguration configuration, IUserRepository userControllerInterface) : ControllerBase
{
    /// <summary>
    /// UserIdとPasswordがあっていれば，Tokenを返します
    /// </summary>
    /// <param name="userLoginModel"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserLoginModel userLoginModel)
    {
        UserModel? user = await userControllerInterface.GetUser(userLoginModel.UserId);
        if (user == null)
        {
            return NotFound("User not found");
        }
        if (PasswordUtil.CompareHash(user.PasswordHash, PasswordUtil.GetPasswordHashFromPepper(user.PasswordSalt, userLoginModel.Password, configuration["Pepper"] ?? throw new Exception("app settingsのPepperがnullです"))))
        {
            var token = tokenService.GenerateToken(
                configuration["Jwt:Key"] ?? throw new Exception("app settingsのJwt:Keyがnullです"),
                user
            );
            return Ok(token);
        }
        else
        {
            return Unauthorized();
        }
    }

}