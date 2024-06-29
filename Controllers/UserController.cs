using System.Text;
using BATTARI_api.Data;
using BATTARI_api.Interfaces;
using BATTARI_api.Models;
using BATTARI_api.Repository;
using Microsoft.AspNetCore.Mvc;
using webUserLoginTest.Util;

namespace BATTARI_api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController(IUserRepository _userRepositoryInterface, ITokenService tokenService, IConfiguration configuration) : ControllerBase
{
    
    //#TODO Exception型定義
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userRegisterModel"></param>
    /// <returns>token
    /// ユーザーIDがすでに存在する場合はConflictステータスを返します
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<string>> CreateUser(UserRegisterModel userRegisterModel)
    {
        //TODO userModelの作成とかはここでやった方がいい気がする（IUserRepositoryはデータベースのアクセスだけやるイメージ）
        if(await _userRepositoryInterface.UserExists(userRegisterModel.UserId))
        {
            return Conflict("User already exists");
        }
        var userModel = await _userRepositoryInterface.CreateUser(userRegisterModel);
        if (userModel == null)
        {
            return BadRequest();
        }
        return tokenService.GenerateToken(configuration["Jwt:Key"] ?? "", userModel);
    }
    
    [HttpPost]
    public async Task<ActionResult<string>> Login(UserLoginModel userLoginModel)
    {
        var userModel = await _userRepositoryInterface.GetUser(userLoginModel.UserId);
        if (userModel == null)
        {
            return NotFound();
        }
        if(PasswordUtil.CompareHash(userModel.PasswordHash, PasswordUtil.GetPasswordHashFromPepper(userModel.PasswordSalt, userLoginModel.Password, "BATTARI")))
        {
            return tokenService.GenerateToken(configuration["Jwt:Key"] ?? "", userModel);
        }
        return Unauthorized();
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<UserModel>> DeleteUser(int id)
    {
        var userModel = await _userRepositoryInterface.GetUser(id);
        if (userModel == null)
        {
            return NotFound();
        }
        _ = _userRepositoryInterface.DeleteUser(userModel.Id);
        return userModel;
    }

    [HttpGet]
    public async Task<ActionResult<UserModel>> GetUsers() {
        var users = await _userRepositoryInterface.GetUsers();
        if(users.Count() == 0) return NotFound();
        return Ok(users);
    }
}