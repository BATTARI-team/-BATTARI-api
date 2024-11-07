class LoginDto
{
    public required string UserId { get; set; }
    public required string Password { get; set; }
}

public class AuthenticatedDto
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
}

public class RefreshTokenDto
{
    public required string RefreshToken { get; set; }
    public required int UserIndex { get; set; }
}
