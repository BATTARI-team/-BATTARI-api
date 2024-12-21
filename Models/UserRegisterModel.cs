namespace BATTARI_api.Models;

public class UserRegisterModel
{
    public required string Name { get; set; }
    public required string Password { get; set; }
    public required string UserId { get; set; }
}