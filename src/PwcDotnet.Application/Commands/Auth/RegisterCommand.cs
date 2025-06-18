namespace PwcDotnet.Application.Commands.Auth;

public record RegisterCommand(string Email, string Password, string FullName)
    : IRequest<TokenDto>;
