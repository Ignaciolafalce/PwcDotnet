namespace PwcDotnet.Domain.AggregatesModel.RentalAggregate;

public class Rental : Entity, IAggregateRoot
{
    public int CustomerId { get; private set; }
    public Customer Customer { get; private set; } = default!;
    public int CarId { get; private set; }
    public Car Car { get; private set; } = default!;
    public RentalPeriod Period { get; private set; }
    public RentalStatus Status { get; private set; }

    protected Rental() { }
    private Rental(int customerId, int carId, RentalPeriod period)
    {
        if (period == null)
            throw new RentalDomainException("Rental period is required");

        CustomerId = customerId;
        CarId = carId;
        Period = period;
        Status = RentalStatus.Active;

        AddDomainEvent(new RentalCreatedDomainEvent(this));
    }

    public static Rental Create(int? customerId, int? carId, RentalPeriod period)
    {
        if (!customerId.HasValue || customerId.Value <= 0)
            throw new RentalDomainException("Customer ID must be valid and/or grater than 0");

        if (!carId.HasValue || carId.Value <= 0)
            throw new RentalDomainException("Car ID must be valid and/or grater than 0");

        if (period == null)
            throw new RentalDomainException("Rental period is required");

        return new Rental(customerId.Value, carId.Value, period);
    }


    public void ChangePeriod(RentalPeriod newPeriod) // Modify the rental period
    {
        if (Status != RentalStatus.Active)
            throw new RentalDomainException("Only active rentals can be modified");

        if (newPeriod == null)
            throw new RentalDomainException("New period cannot be null");

        Period = newPeriod;
    }

    public void ChangeCar(int newCarId)
    {
        if (Status != RentalStatus.Active)
            throw new RentalDomainException("Only active rentals can be modified");

        CarId = newCarId;
    }

    public void Cancel()
    {
        if (!CanBeCancelled())
            throw new RentalDomainException("Rental cannot be cancelled once it has started");

        Status = RentalStatus.Cancelled;
    }

    public bool CanBeCancelled() => Period.Start > DateTime.UtcNow;
}
