namespace PwcDotnet.WebAPI.Apis.Services;

public class CarServices
{
    public required ISender Mediator { get; init; }
    public required ILogger<CarServices> Logger { get; init; }

    public CarServices(IMediator mediator, ILogger<CarServices> logger)
    {
        Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}