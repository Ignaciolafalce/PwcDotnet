using Microsoft.AspNetCore.Identity;

namespace PwcDotnet.Application.Common.Auth.IdentityEntities;

public class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
}
