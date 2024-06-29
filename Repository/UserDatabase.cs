using System.Drawing.Printing;
using BATTARI_api.Data;
using BATTARI_api.Interfaces;
using BATTARI_api.Migrations;
using BATTARI_api.Models;
using Microsoft.EntityFrameworkCore;
using webUserLoginTest.Util;

namespace BATTARI_api.Repository;

public class UserDatabase(UserContext userContext) : IUserRepository
{
    const string _pepper = "BATTARI";
    private UserContext _userContext = userContext;

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
            UserId = userRegisterModel.UserId,
            Name = userRegisterModel.Name,
            PasswordHash = PasswordUtil.GetPasswordHashFromPepper(_salt, userRegisterModel.Password, _pepper),
            PasswordSalt = _salt,
            Created = _created
        };
        
        var result = await _userContext.AddAsync(userModel);
        Console.WriteLine(result);

        await _userContext.SaveChangesAsync();
        return userModel;
    }

    public async Task<UserModel?> GetUser(string userId)
    {
        Console.WriteLine(userId);
        UserModel user = await _userContext.Users.Where<UserModel>(x => x.UserId == userId).FirstAsync();
        Console.WriteLine(user.Created);
        return user;
    }

    public async Task<UserModel> DeleteUser(int id)
    {
        UserModel user = await _userContext.Users.FindAsync(id);
        if(user != null) {
            _userContext.Users.Remove(user);
            await _userContext.SaveChangesAsync();
            return user;
        } else {
            throw new Exception("User not found");
        }
    }

    public async Task<IEnumerable<UserModel>> GetUsers()
    {
        return await _userContext.Users.ToListAsync();
    }
}