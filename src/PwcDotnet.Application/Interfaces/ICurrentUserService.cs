namespace PwcDotnet.Application.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? Expiration { get; }
    string? Email { get; }
    bool IsInRole(string roleName);
}
