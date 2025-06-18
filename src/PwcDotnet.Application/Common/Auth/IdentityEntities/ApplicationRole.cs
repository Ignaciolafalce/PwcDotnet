using Microsoft.AspNetCore.Identity;

namespace PwcDotnet.Application.Common.Auth.IdentityEntities;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole() : base() { }
    public ApplicationRole(string roleName) : base(roleName) { }

}
