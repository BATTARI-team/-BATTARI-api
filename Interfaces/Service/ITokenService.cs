using BATTARI_api.Models;

public interface ITokenService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="userModel"></param>
    /// <param name="expires"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="userModel"></param>
    /// <param name="expires"></param>
    /// <returns></returns>
    public string GenerateToken(string key, UserModel userModel, DateTime? expires = null);
}