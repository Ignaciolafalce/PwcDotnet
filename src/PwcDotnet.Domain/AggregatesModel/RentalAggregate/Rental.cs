namespace PwcDotnet.Domain.AggregatesModel.RentalAggregate;

public class Rental : Entity, IAggregateRoot
{
    public int CustomerId { get; private set; }
    public int CarId { get; private set; }
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

    public static Rental Create(int customerId, int carId, RentalPeriod period)
    {
        if (customerId == int.Empty)
            throw new RentalDomainException("Customer ID is required");

        if (carId == int.Empty)
            throw new RentalDomainException("Car ID is required");

        if (period == null)
            throw new RentalDomainException("Rental period is required");

        return new Rental(customerId, carId, period);
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
