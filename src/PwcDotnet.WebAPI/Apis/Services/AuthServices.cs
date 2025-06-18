namespace PwcDotnet.WebAPI.Apis.Services;

public class AuthServices
{
    public ISender Mediator { get; init; }
    public ILogger<AuthServices> Logger { get; init; }
    public ICurrentUserService CurrentUser { get; init; }

    public AuthServices(IMediator mediator, ILogger<AuthServices> logger, ICurrentUserService currentUser)
    {
        Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        CurrentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }
}
