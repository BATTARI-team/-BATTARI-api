using BATTARI_api.Data;
using BATTARI_api.Interfaces;
using BATTARI_api.Models;
using BATTARI_api.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BATTARI_api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase
{
    
    private static IUserControllerInterface _userControllerInterface = new UserDatabase(new UserContext());
    [HttpPost]
    public async Task<ActionResult<UserModel>> CreateUser(UserRegisterModel userRegisterModel)
    {
        var userModel = await _userControllerInterface.CreateUser(userRegisterModel);
        return userModel;
    }
}