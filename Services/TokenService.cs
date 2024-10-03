using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BATTARI_api.Models;
using Microsoft.IdentityModel.Tokens;

namespace BATTARI_api.Services;

public class TokenService : ITokenService
{
  /// <summary>
  /// JWTTokenを生成します
  /// key, issuer, audienceはappsettings.jsonから取得します
  /// </summary>
  /// <param name="key"></param>
  /// 16バイト以上
  /// <param name="userModel"></param>
  /// JWTの有効期限
  /// <param name="expires"></param>
  /// <returns></returns>
  public string GenerateToken(string key, UserModel userModel,
                              DateTime? expires = null)
  {
    // 暗号化アルゴリズム(鍵の生成)
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    // 署名の作成
    var credentials =
        new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new[] {
      new Claim(JwtRegisteredClaimNames.Iss, "BATTARI-team"),
      new Claim(JwtRegisteredClaimNames.NameId, userModel.UserId),
      new Claim(JwtRegisteredClaimNames.Name, userModel.Name),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

    var token = new JwtSecurityToken(
        claims: claims, expires: expires ?? DateTime.Now.AddDays(1),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
