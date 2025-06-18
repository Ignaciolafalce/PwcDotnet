using Microsoft.AspNetCore.Identity;
using PwcDotnet.Application.Common.Auth.IdentityEntities;
using PwcDotnet.Application.Common.Auth.Interfaces;
using PwcDotnet.Application.Common.Exceptions;
namespace PwcDotnet.Application.Commands.Auth;

public class TokenCommandHandler : IRequestHandler<TokenCommand, TokenDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenGenerator _tokenGenerator;

    public TokenCommandHandler(UserManager<ApplicationUser> userManager, ITokenGenerator tokenGenerator)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _tokenGenerator = tokenGenerator;
    }

    public async Task<TokenDto> Handle(TokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new ForbiddenAccessException("Invalid credentials");
        }

        var roles = await _userManager.GetRolesAsync(user);

        return _tokenGenerator.Generate(user, roles);

    }
}