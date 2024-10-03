using System.ComponentModel.DataAnnotations;

public class RefreshTokenModel
{
  public string Token { get; set; }
  public DateTime Expires { get; set; }
  public DateTime Created { get; set; }
  public bool IsActive { get; set; }
  public int UserId { get; set; }
  [Key]
  public int Id { get; set; }

  public RefreshTokenModel(string token, DateTime expires, DateTime created,
                           int userId, bool isActive)
  {
    Token = token;
    Expires = expires;
    Created = created;
    IsActive = isActive;
    UserId = userId;
  }
}
