using Microsoft.AspNetCore.Identity;
using PwcDotnet.Application.Common.Auth;
using PwcDotnet.Application.Common.Auth.IdentityEntities;
using PwcDotnet.Application.Common.Auth.Interfaces;

namespace PwcDotnet.Application.Commands.Auth;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, TokenDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenGenerator _tokenGenerator;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager, ITokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<TokenDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ApplicationException($"User registration failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, RoleConstants.Manager); // Default role assignment, can be other

        var roles = await _userManager.GetRolesAsync(user);

        return _tokenGenerator.Generate(user, roles);
    }
}