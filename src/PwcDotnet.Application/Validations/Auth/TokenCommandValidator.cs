using PwcDotnet.Application.Commands.Auth;

namespace PwcDotnet.Application.Validations.Auth;

public class TokenCommandValidator : AbstractValidator<TokenCommand>
{
    public TokenCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}