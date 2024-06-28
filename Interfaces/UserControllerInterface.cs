using BATTARI_api.Models;

namespace BATTARI_api.Interfaces;

public interface IUserControllerInterface
{
    Task<UserModel?> GetUser(int id);
    Task<UserModel?> GetUser(string userId);
    Task<IEnumerable<UserModel>> GetUsers();
    Task<UserModel> CreateUser(UserRegisterModel userRegisterModel);
    Task<UserModel> DeleteUser(int id);
}