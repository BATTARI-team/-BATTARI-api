namespace BATTARI_api.Interfaces;

public interface IRefreshTokensRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">トークンが見つからなかった場合</exception>
    Task<RefreshTokenModel> GetByToken(string token);
    Task Add(RefreshTokenModel refreshToken);
    Task Deactivate(int refreshTokenId);
}