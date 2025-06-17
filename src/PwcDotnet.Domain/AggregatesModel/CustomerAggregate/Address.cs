namespace PwcDotnet.Domain.AggregatesModel.CustomerAggregate;

public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string Country { get; }

    public Address(string street, string city, string country)
    {
        Street = street;
        City = city;
        Country = country;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return Country;
    }
}
