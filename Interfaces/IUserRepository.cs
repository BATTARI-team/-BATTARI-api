using BATTARI_api.Models;

namespace BATTARI_api.Interfaces;

public interface IUserRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// indexIdでユーザーを取得します
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<UserModel?> GetUser(int id);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// userIdでユーザーを取得します
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<UserModel?> GetUser(string userId);
    Task<IEnumerable<UserModel>> GetUsers();
    Task<UserModel?> CreateUser(UserModel userRegisterModel);
    Task<UserModel> DeleteUser(int id);
    Task<bool> UserExists(string userId);
}