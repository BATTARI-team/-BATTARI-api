using BATTARI_api.Interfaces;
using BATTARI_api.Interfaces.Service;
using BATTARI_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using webUserLoginTest.Util;

namespace BATTARI_api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]/[action]")]
public class UserController
(IUserRepository userRepositoryInterface, ITokenService tokenService,
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
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticatedDto>> CreateUser(
        UserRegisterModel userRegisterModel)
    {
        if (await userRepositoryInterface.UserExists(userRegisterModel.UserId))
        {
            return Conflict("User already exists");
        }

        DateTime now = DateTime.Now;
        // #TODO なんかやばそう
        byte[] salt = PasswordUtil.GetInitialPasswordSalt(now.ToString());
        UserModel user =
            new UserModel()
            {
                UserId = userRegisterModel.UserId,
                Name = userRegisterModel.Name,
                PasswordHash = PasswordUtil.GetPasswordHashFromPepper(
                                  salt, userRegisterModel.Password, configuration["Pepper"] ?? throw new Exception("app settingsのPepperがnullです")),
                PasswordSalt = salt,
                Created = DateTime.Now
            };

        var userModel = await userRepositoryInterface.CreateUser(user);
        if (userModel == null)
        {
            return BadRequest();
        }
        String refreshToken =
            await tokenService.GenerateAndSaveRefreshToken(userModel);
        String token =
            tokenService.GenerateToken(configuration["Jwt:Key"] ?? throw new Exception("app settingsのJwt:Keyがnullです"), userModel);
        return new AuthenticatedDto { Token = token, RefreshToken = refreshToken };
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<bool> UserExists(string userId)
    {
        return await userRepositoryInterface.UserExists(userId);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticatedDto>> Login(
        UserLoginModel userLoginModel)
    {
        var userModel =
            await userRepositoryInterface.GetUser(userLoginModel.UserId);
        if (userModel == null)
        {
            return NotFound();
        }
        if (PasswordUtil.CompareHash(
                userModel.PasswordHash,
                PasswordUtil.GetPasswordHashFromPepper(
                    userModel.PasswordSalt, userLoginModel.Password, configuration["Pepper"] ?? throw new Exception("app settingsのPepperがnullです"))))
        {
            return new AuthenticatedDto
            {
                Token = tokenService.GenerateToken(configuration["Jwt:Key"] ?? throw new Exception("app settingsのJwt:Keyがnullです"),
                                                 userModel),
                RefreshToken =
                  await tokenService.GenerateAndSaveRefreshToken(userModel),
            };
        }

        return Unauthorized("パスワードかuserIdが間違っています");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<string>> RefreshToken(
        RefreshTokenDto refreshToken)
    {
        RefreshTokenModel refreshTokenModel;
            var transaction = SentrySdk.StartTransaction("RefreshToken", "RefreshToken");
        try
        {
            refreshTokenModel =
                await tokenService.ValidateRefreshToken(refreshToken.RefreshToken);
        }
        catch (KeyNotFoundException e)
        {
            transaction.Finish();
            return NotFound(e);
        }
        catch (Exception e)
        {
            transaction.Finish();
            return BadRequest(e.ToString());
        }

        var userModel = await userRepositoryInterface.GetUser(refreshTokenModel.UserId);
        if (userModel == null)
        {
            transaction.Finish();
            return NotFound("User not found");
        }
        if (userModel.Id != refreshToken.UserIndex)
        {
            transaction.Finish();
            return BadRequest("Invalid user");
        }

        string token =
            tokenService.GenerateToken(configuration["Jwt:Key"] ?? throw new Exception("app settingsのJwt:Keyがnullです"), userModel);
        transaction.Finish();
        return token;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<UserModel>> DeleteUser(int id)
    {
        var userModel = await userRepositoryInterface.GetUser(id);
        if (userModel == null)
        {
            return NotFound();
        }
        _ = userRepositoryInterface.DeleteUser(userModel.Id);
        return userModel;
    }

    [HttpGet]
    public async Task<ActionResult<UserModel>> GetUsers()
    {
        var users = await userRepositoryInterface.GetUsers();
        if (!users.Any())
            return NotFound();
        return Ok(users);
    }

    [HttpPut]
    public async Task<ActionResult<UserModel>> ChangeNickname(string userName,
                                                              string userId)
    {
        var user = await userRepositoryInterface.ChangeNickname(nickname: userName,
                                                                 userId: userId);
        if (user == null)
            return BadRequest();
        return user;
    }

    [HttpPut]
    public async Task<ActionResult<UserDto>> GetUser(int userIndex)
    {
        var userModel = await userRepositoryInterface.GetUser(userIndex);
        if (userModel != null)
        {
            var user = new UserDto()
            {
                Name = userModel.Name,
                UserId = userModel.UserId,
                Id = userModel.Id,
            };

            return user;
        }
        else
        {
            return NotFound("User not found");
        }
    }

    [HttpPut]
    public async Task<ActionResult<UserDto>> GetUserByUserId(string userId)
    {
        var userModel = await userRepositoryInterface.GetUser(userId);
        if (userModel != null)
        {
            var user = new UserDto()
            {
                Name = userModel.Name,
                UserId = userModel.UserId,
                Id = userModel.Id,
            };

            return user;
        }
        return NotFound("User not found");
    }
}
