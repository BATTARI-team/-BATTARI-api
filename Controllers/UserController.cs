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
public class UserController
(IUserRepository _userRepositoryInterface, ITokenService tokenService,
 IConfiguration configuration)
    : ControllerBase
{
    // #TODO Exception型定義
    /// <summary>
    ///
    /// </summary>
    /// <param name="userRegisterModel"></param>
    /// <returns>token
    /// ユーザーIDがすでに存在する場合はConflictステータスを返します
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<string>> CreateUser(
        UserRegisterModel userRegisterModel)
    {
        if (await _userRepositoryInterface.UserExists(userRegisterModel.UserId))
        {
            return Conflict("User already exists");
        }

        // TODO コアロジックなので，別の場所に移動した方がいい
        DateTime now = DateTime.Now;
        byte[] _salt = PasswordUtil.GetInitialPasswordSalt(now.ToString());
        UserModel user =
            new UserModel()
            {
                UserId = userRegisterModel.UserId,
                Name = userRegisterModel.Name,
                PasswordHash = PasswordUtil.GetPasswordHashFromPepper(
                                  _salt, userRegisterModel.Password, "BATTARI"),
                PasswordSalt = _salt,
                Created = DateTime.Now
            };

        var userModel = await _userRepositoryInterface.CreateUser(user);
        if (userModel == null)
        {
            return BadRequest();
        }
        return tokenService.GenerateToken(configuration["Jwt:Key"] ?? "",
                                          userModel);
    }

    [HttpGet]
    public async Task<bool> UserExists(string userId)
    {
        return await _userRepositoryInterface.UserExists(userId);
    }

    [HttpPost]
    public async Task<ActionResult<AuthenticatedDto>> Login(
        UserLoginModel userLoginModel)
    {
        var userModel =
            await _userRepositoryInterface.GetUser(userLoginModel.UserId);
        if (userModel == null)
        {
            return NotFound();
        }
        if (PasswordUtil.CompareHash(
                userModel.PasswordHash,
                PasswordUtil.GetPasswordHashFromPepper(
                    userModel.PasswordSalt, userLoginModel.Password, "BATTARI")))
        {
            // return tokenService.GenerateToken(configuration["Jwt:Key"] ?? "",
            //                                   userModel);
            return new AuthenticatedDto
            {
                Token = tokenService.GenerateToken(configuration["Jwt:Key"] ?? "",
                                                 userModel),
                RefreshToken =
                  await tokenService.GenerateAndSaveRefreshToken(userModel),
            };
        }

        return Unauthorized();
    }

    [HttpPut]
    public async Task<ActionResult<string>> RefreshToken(string refreshToken)
    {
        RefreshTokenModel refreshTokenModel;
        try
        {
            refreshTokenModel = await tokenService.ValidateRefreshToken(refreshToken);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }

        UserModel? userModel;

        userModel = await _userRepositoryInterface.GetUser(refreshTokenModel.UserId);
        if(userModel == null)
        {
            return NotFound("User not found");
        }

        string token = tokenService.GenerateToken(configuration["Jwt:Key"] ?? "", userModel);
        return token;
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
    public async Task<ActionResult<UserModel>> GetUsers()
    {
        var users = await _userRepositoryInterface.GetUsers();
        if (users.Count() == 0)
            return NotFound();
        return Ok(users);
    }

    [HttpPut]
    public async Task<ActionResult<UserModel>> ChangeNickname(string userName,
                                                              string userId)
    {
        var user = await _userRepositoryInterface.ChangeNickname(nickname: userName,
                                                                 userId: userId);
        if (user == null)
            return BadRequest();
        return user;
    }
}
