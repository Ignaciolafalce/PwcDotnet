namespace PwcDotnet.Domain.AggregatesModel.CustomerAggregate;

public class Customer : Entity, IAggregateRoot
{
    public string IdentityGuid { get; private set; } = default!;
    public string FullName { get; private set; }
    public Address Address { get; private set; }
    public string Email { get; private set; }

    protected Customer() { }

    public Customer(string fullName, Address address, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new RentalDomainException("Customer full name is required");

        FullName = fullName;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Email = email ?? throw new RentalDomainException("Customer email is required");
    }

    public void UpdateAddress(Address newAddress)
    {
        Address = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
    }
    public void LinkToUser(string userId)
    {
        IdentityGuid = userId;
    }
}
