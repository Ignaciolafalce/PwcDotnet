namespace PwcDotnet.Domain.AggregatesModel.CarAggregate;

public class Car : Entity, IAggregateRoot
{
    public string Model { get; private set; }
    public CarType Type { get; private set; }

    private readonly List<Service> _services = new();
    public IReadOnlyCollection<Service> Services => _services;

    protected Car() { }

    public Car(string model, CarType type)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new RentalDomainException("Car model is required");

        Model = model;
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }

    public void ScheduleService(DateTime date)
    {
        _services.Add(new Service(date));
    }

    public bool HasServiceIn(DateRange range)
    {
        return _services.Any(s => range.Contains(s.Date));
    }

    public bool IsAvailable(DateRange range)
    {
        return !HasServiceIn(range);
    }
}
