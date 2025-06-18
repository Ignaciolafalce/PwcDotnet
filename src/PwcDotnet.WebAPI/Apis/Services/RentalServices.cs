namespace PwcDotnet.WebAPI.Apis.Services;

public class RentalServices
{
    public required ISender Mediator { get; set; }
    public required ILogger<CustomerServices> Logger { get; set; }

    public RentalServices(IMediator mediator, ILogger<CustomerServices> logger)
    {
        Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
