namespace PwcDotnet.Domain.AggregatesModel.CarAggregate;

public class Car : Entity, IAggregateRoot
{
    public string IdentityGuid { get; private set; } = default!;
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public int LocationId { get; private set; }
    public Location Location { get; private set; } = default!;
    public CarType Type { get; private set; }

    private readonly List<Service> _services = new();
    public IReadOnlyCollection<Service> Services => _services;

    private Car() { }

    public Car(string brand, string model, CarType type, int locationId)
    {
        if (string.IsNullOrWhiteSpace(brand))
            throw new RentalDomainException("Car Brand is required");

        if (string.IsNullOrWhiteSpace(model))
            throw new RentalDomainException("Car model is required");

        Brand = brand;
        Model = model;
        Type = type ?? throw new ArgumentNullException(nameof(type));

        LocationId = locationId > 0 ? locationId : throw new RentalDomainException("Location ID must be greater than zero");
    }

    // this could be use in later functionaties out of the scope of the challenge
    public void ScheduleService(DateTime date)
    {
        _services.Add(new Service(date));
    }

    public bool HasServiceIn(DateRange range)
    {
        return _services.Any(s => range.Contains(s.Date));
    }

    public bool IsAvailableOfServices(DateRange range)
    {
        return !HasServiceIn(range);
    }
    public void LinkToUser(string userId)
    {
        IdentityGuid = userId;
    }
}
