class LoginDto
{
    public string UserId { get; set; }
    public string Password { get; set; }
}

public class AuthenticatedDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
