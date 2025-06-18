namespace PwcDotnet.Domain.AggregatesModel.CarAggregate;

public class Location : Entity
{
    public string Name { get; private set; } = default!;

    public Location(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    private Location() { } // EF
}
