
using Microsoft.Extensions.Logging;

namespace PwcDotnet.Application.DomainEventHandlers;

public class RentalCreatedDomainEventHandler : INotificationHandler<RentalCreatedDomainEvent>
{

    // repositories, services, or other dependencies can be injected here
    private readonly ILogger<RentalCreatedDomainEventHandler> _logger;

    public RentalCreatedDomainEventHandler(ILogger<RentalCreatedDomainEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(RentalCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Invoke durable function with azurite send email!
        return Task.CompletedTask;
    }
}
