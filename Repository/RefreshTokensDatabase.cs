using BATTARI_api.Interfaces;
using BATTARI_api.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace BATTARI_api.Repository;

class RefreshTokenDatabase(UserContext context) : IRefreshTokensRepository
{
    public async Task Add(RefreshTokenModel refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }
    public async Task Deactivate(int refreshTokenId)
    {
        var refreshToken = await context.RefreshTokens.FindAsync(refreshTokenId);
        if (refreshToken == null)
            throw new KeyNotFoundException("RefreshToken not found");
        refreshToken.IsActive = false;
        await context.SaveChangesAsync();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">指定されたリフレッシュトークンが保存されていなかった場合</exception>
    public async Task<RefreshTokenModel> GetByToken(string token)
    {
        RefreshTokenModel? refreshToken =
            await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        if (refreshToken == null)
            throw new KeyNotFoundException("RefreshToken not found");
        return refreshToken;
    }
}