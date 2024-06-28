using BATTARI_api.Models;

public interface ITokenService
{
    public string GenerateToken(string key, UserModel userModel, DateTime? expires = null);
}