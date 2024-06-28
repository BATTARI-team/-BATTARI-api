using BATTARI_api.Models;

namespace BATTARI_api.Interfaces;

public interface IUserControllerInterface
{
    Task<UserModel?> GetUser(int id);
    Task<UserModel> CreateUser(UserRegisterModel userRegisterModel);
}