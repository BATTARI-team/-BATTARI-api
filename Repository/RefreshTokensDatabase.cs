using BATTARI_api.Data;
using Microsoft.EntityFrameworkCore;

class RefreshTokenDatabase : IRefreshTokensRepository
{
  private readonly UserContext _context;
  public RefreshTokenDatabase(UserContext context) { _context = context; }
  public async Task Add(RefreshTokenModel refreshToken)
  {
    await _context.RefreshTokens.AddAsync(refreshToken);
    await _context.SaveChangesAsync();
  }
  public async Task Deactivate(int refreshTokenId)
  {
    var refreshToken = await _context.RefreshTokens.FindAsync(refreshTokenId);
    if (refreshToken == null)
      throw new KeyNotFoundException("RefreshToken not found");
    refreshToken.IsActive = false;
    await _context.SaveChangesAsync();
  }
  public async Task<RefreshTokenModel> GetByToken(string token)
  {
    var refreshToken =
        await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
    if (refreshToken == null)
      throw new KeyNotFoundException("RefreshToken not found");
    return refreshToken;
  }
}
