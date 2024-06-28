using BATTARI_api.Data;
using BATTARI_api.Interfaces;
using BATTARI_api.Models;
using webUserLoginTest.Util;

namespace BATTARI_api.Repository;

public class UserDatabase(UserContext userContext) : IUserControllerInterface
{
    const string _pepper = "BATTARI";
    private readonly UserContext _userContext = userContext;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<UserModel?> GetUser(int id)
    {
        return await _userContext.Users.FindAsync(id);
    }

    public async Task<UserModel> CreateUser(UserRegisterModel userRegisterModel)
    {
        //ここら辺はコアロジック？
        DateTime _created = DateTime.Now;
        byte[] _salt = PasswordUtil.GetInitialPasswordSalt(_created.ToString());
        UserModel userModel = new UserModel()
        {
            Name = userRegisterModel.Name,
            PasswordHash = PasswordUtil.GetPasswordHashFromPepper(_salt, userRegisterModel.Password, _pepper),
            PasswordSalt = _salt,
            Created = _created
        };
        
        var result = await _userContext.AddAsync(userModel);
        Console.WriteLine(result);

        return userModel;
    }
}