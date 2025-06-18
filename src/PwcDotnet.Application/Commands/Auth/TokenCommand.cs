namespace PwcDotnet.Application.Commands.Auth;

public record TokenCommand(string Email, string Password) : IRequest<TokenDto>;