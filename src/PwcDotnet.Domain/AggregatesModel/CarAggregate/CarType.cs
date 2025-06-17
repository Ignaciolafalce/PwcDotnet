namespace PwcDotnet.Domain.AggregatesModel.CarAggregate;

public class CarType : ValueObject
{
    public string Name { get; }

    public CarType(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new RentalDomainException("Car type name is required");

        Name = name;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}