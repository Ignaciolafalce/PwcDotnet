namespace PwcDotnet.Application.DTOs;

public class TokenDto
{
    public string AccessToken { get; set; } = default!;
    public DateTime Expiration { get; init; }
    public string Email { get; init; } = string.Empty;
}