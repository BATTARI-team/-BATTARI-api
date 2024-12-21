using BATTARI_api.Models;

namespace BATTARI_api.Interfaces.Service;

public interface ITokenService
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="userModel"></param>
    /// <param name="expires"></param>
    /// <returns></returns>
    public string GenerateToken(string key, UserModel userModel,
        DateTime? expires = null);
    public Task<string> GenerateAndSaveRefreshToken(UserModel userModel);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <exception cref="KeyNotFoundException">トークンが存在しなかった時</exception>
    /// <returns></returns>
    public Task<RefreshTokenModel> ValidateRefreshToken(string refreshToken);
}