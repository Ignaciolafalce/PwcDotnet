
using Microsoft.Extensions.Logging;
using PwcDotnet.Application.Interfaces;

namespace PwcDotnet.Application.DomainEventHandlers;

public class RentalCreatedDomainEventHandler : INotificationHandler<RentalCreatedDomainEvent>
{

    // repositories, services, or other dependencies can be injected here
    private readonly ILogger<RentalCreatedDomainEventHandler> _logger;
    private readonly INotificationService _notificationService;

    public RentalCreatedDomainEventHandler(ILogger<RentalCreatedDomainEventHandler> logger, INotificationService notificationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _notificationService = notificationService;
    }

    public Task Handle(RentalCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _notificationService.NotifyRentalCreatedAsync(notification.RentalId, notification.CustomerId, notification.StartDate, notification.EndDate);
        return Task.CompletedTask;
    }
}
