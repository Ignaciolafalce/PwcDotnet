using PwcDotnet.Application.Common.Auth.IdentityEntities;

namespace PwcDotnet.Application.Common.Auth.Interfaces;

public interface ITokenGenerator
{
    TokenDto Generate(ApplicationUser user, IList<string> roles);
}