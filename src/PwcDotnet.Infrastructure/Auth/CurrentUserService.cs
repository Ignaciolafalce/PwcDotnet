using Microsoft.AspNetCore.Http;
using PwcDotnet.Application.Interfaces;
using System.Security.Claims;

namespace PwcDotnet.Infrastructure.Auth;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Sid);

    public string? UserName =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    public string? Email =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    public string? Expiration => _httpContextAccessor.HttpContext?.User?.FindFirstValue("exp");

    public bool IsInRole(string roleName) =>
        _httpContextAccessor.HttpContext?.User?.IsInRole(roleName) ?? false;
}