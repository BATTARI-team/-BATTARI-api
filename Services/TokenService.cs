using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BATTARI_api.Services;

public class TokenService
{
    /// <summary>
    /// JWTTokenを生成します
    /// </summary>
    /// <param name="key"></param>
    /// 16バイト以上
    /// <param name="issuer"></param>
    /// <param name="audience"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    public static string GenerateToken(string key, string issuer, string audience, string userid)
    {
        Console.WriteLine("generate token" + key + issuer + audience + userid);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userid),
            new Claim(JwtRegisteredClaimNames.Name, "test"),
            new Claim(JwtRegisteredClaimNames.Email, "test@test.com"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddYears(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}