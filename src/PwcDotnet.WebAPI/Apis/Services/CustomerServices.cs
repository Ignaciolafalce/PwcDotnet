namespace PwcDotnet.WebAPI.Apis.Services;

public class CustomerServices
{
    public required ISender Mediator { get; set; }
    public required ILogger<CustomerServices> Logger { get; set; }

    public CustomerServices(IMediator mediator, ILogger<CustomerServices> logger)
    {
        Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
