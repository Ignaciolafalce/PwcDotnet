using FluentAssertions;
using PwcDotnet.Domain.AggregatesModel.RentalAggregate;
using PwcDotnet.Domain.Events;

namespace PwcDotnet.UnitTests.Domain.Rentals;

public class RentalCreatedDomainEventTests
{
    [Fact]
    public void Should_Create_Event_From_Rental()
    {
        var period = new RentalPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        var rental = Rental.Create(3, 4, period);

        var domainEvent = new RentalCreatedDomainEvent(rental);

        domainEvent.RentalId.Should().Be(rental.Id);
        domainEvent.CustomerId.Should().Be(3);
        domainEvent.CarId.Should().Be(4);
        domainEvent.StartDate.Should().Be(period.Start);
        domainEvent.EndDate.Should().Be(period.End);
        domainEvent.Rental.Should().Be(rental);
    }
}
