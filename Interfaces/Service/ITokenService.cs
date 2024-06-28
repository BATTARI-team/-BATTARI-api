public interface ITokenService
{
    public string GenerateToken(string key, string issuer, string audience, string userid);
}