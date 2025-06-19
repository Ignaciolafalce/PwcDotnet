using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PwcDotnet.Application.DomainEventHandlers;
using PwcDotnet.Domain.AggregatesModel.RentalAggregate;
using PwcDotnet.Domain.Events;

namespace PwcDotnet.UnitTests.Application.Rentals;
public class RentalCreatedDomainEventHandlerTests
{
    [Fact]
    public async Task Handle_Should_Complete_Without_Exception()
    {
        var loggerMock = new Mock<ILogger<RentalCreatedDomainEventHandler>>();
        var handler = new RentalCreatedDomainEventHandler(loggerMock.Object);

        var rental = Rental.Create(1, 2, new RentalPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(3)));
        var domainEvent = new RentalCreatedDomainEvent(rental);

        var act = async () => await handler.Handle(domainEvent, default);

        await act.Should().NotThrowAsync();

        loggerMock.VerifyNoOtherCalls(); // currently no logs, but keeps coverage intact
    }
}