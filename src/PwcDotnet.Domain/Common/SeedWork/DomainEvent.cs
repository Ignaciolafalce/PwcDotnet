namespace PwcDotnet.Domain.Common.SeedWork;

public class DomainEvent : INotification
{
    public bool IsPublished { get; set; }
    public DateTimeOffset DateOcurred { get; set; } = DateTime.UtcNow;

    protected DomainEvent()
    {
        DateOcurred = DateTime.UtcNow;
    }
}
