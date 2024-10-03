interface IRefreshTokensRepository
{
  Task<RefreshTokenModel> GetByToken(string token);
  Task Add(RefreshTokenModel refreshToken);
  Task Deactivate(int refreshTokenId);
}
