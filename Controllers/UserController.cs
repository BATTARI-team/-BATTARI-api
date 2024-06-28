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
public class UserController : ControllerBase
{
    
    private static IUserControllerInterface _userControllerInterface = new UserDatabase(new UserContext());
    //#TODO Exception型定義
    [HttpPost]
    public async Task<ActionResult<UserModel>> CreateUser(UserRegisterModel userRegisterModel)
    {
        var userModel = await _userControllerInterface.CreateUser(userRegisterModel);
        return userModel;
    }
    
    [HttpPost]
    public async Task<ActionResult<UserModel>> Login(UserLoginModel userLoginModel)
    {
        var userModel = await _userControllerInterface.GetUser(userLoginModel.UserId);
        if (userModel == null)
        {
            return NotFound();
        }
        if (Encoding.Unicode.GetString(userModel.PasswordHash) == Encoding.Unicode.GetString(PasswordUtil.GetPasswordHashFromPepper(userModel.PasswordSalt, userLoginModel.Password, "BATTARI")))
        {
            return userModel;
        }
        return Unauthorized();
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<UserModel>> DeleteUser(int id)
    {
        var userModel = await _userControllerInterface.GetUser(id);
        if (userModel == null)
        {
            return NotFound();
        }
        _ = _userControllerInterface.DeleteUser(userModel.Id);
        return userModel;
    }

    [HttpGet]
    public async Task<ActionResult<UserModel>> GetUsers() {
        var users = await _userControllerInterface.GetUsers();
        if(users.Count() == 0) return NotFound();
        return Ok(users);
    }
}