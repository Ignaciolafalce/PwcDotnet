namespace PwcDotnet.Domain.Events
{

    public class RentalCreatedDomainEvent : DomainEvent
    {
        public Rental Rental { get; }
        public int RentalId { get; }
        public int CustomerId { get; }
        public int CarId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public RentalCreatedDomainEvent(Rental rental)
        {
            Rental = rental;
            RentalId = rental.Id;
            CustomerId = rental.CustomerId;
            CarId = rental.CarId;
            StartDate = rental.Period.Start;
            EndDate = rental.Period.End;
        }
    }
}