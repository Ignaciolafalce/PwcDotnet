using FluentAssertions;
using Moq;
using PwcDotnet.Application.Commands;
using PwcDotnet.Application.Interfaces;
using PwcDotnet.Domain.AggregatesModel.CarAggregate;
using PwcDotnet.Domain.AggregatesModel.RentalAggregate;
using PwcDotnet.Domain.Exceptions;
using Xunit;

namespace PwcDotnet.UnitTests.Application.Rentals;
public class RegisterRentalCommandHandlerTests
{
    private readonly Mock<IRentalRepository> _rentalRepoMock = new();
    private readonly Mock<ICarRepository> _carRepoMock = new();
    private readonly Mock<ICurrentUserService> _currentUserMock = new();
    private readonly RegisterRentalCommandHandler _handler;

    public RegisterRentalCommandHandlerTests()
    {
        _currentUserMock.Setup(x => x.UserId).Returns("user-guid");
        _handler = new RegisterRentalCommandHandler(_rentalRepoMock.Object, _carRepoMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Rental_When_Car_Is_Available()
    {
        // Arrange
        var command = new RegisterRentalCommand(1, 1, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));
        var car = new Car("Ford", "Focus", new CarType("Sedan"), 1);
        _carRepoMock.Setup(r => r.GetByIdAsync(command.CarId)).ReturnsAsync(car);
        _rentalRepoMock.Setup(r => r.IsCarReservedInPeriodAsync(command.CarId, It.IsAny<RentalPeriod>(), null)).ReturnsAsync(false);

        _rentalRepoMock.Setup(r => r.AddAsync(It.IsAny<Rental>()));
        _rentalRepoMock.Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType(typeof(int));
        _carRepoMock.Verify(x => x.GetByIdAsync(command.CarId), Times.Once);
        _rentalRepoMock.Verify(x => x.IsCarReservedInPeriodAsync(command.CarId, It.IsAny<RentalPeriod>(), null), Times.Once);
        _rentalRepoMock.Verify(x => x.AddAsync(It.IsAny<Rental>()), Times.Once);
        _rentalRepoMock.Verify(x => x.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Car_Is_Reserved()
    {
        var command = new RegisterRentalCommand(1, 1, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));
        var car = new Car("Ford", "Focus", new CarType("Sedan"), 1);
        _carRepoMock.Setup(r => r.GetByIdAsync(command.CarId)).ReturnsAsync(car);
        _rentalRepoMock.Setup(r => r.IsCarReservedInPeriodAsync(command.CarId, It.IsAny<RentalPeriod>(), null)).ReturnsAsync(true);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<RentalDomainException>()
                  .WithMessage("*already reserved*");

        _carRepoMock.Verify(x => x.GetByIdAsync(command.CarId), Times.Once);
        _rentalRepoMock.Verify(x => x.IsCarReservedInPeriodAsync(command.CarId, It.IsAny<RentalPeriod>(), null), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Car_Has_Service()
    {
        var command = new RegisterRentalCommand(1, 1, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));
        var car = new Car("Ford", "Focus", new CarType("Sedan"), 1);
        car.ScheduleService(command.StartDate);

        _carRepoMock.Setup(r => r.GetByIdAsync(command.CarId)).ReturnsAsync(car);
        _rentalRepoMock.Setup(r => r.IsCarReservedInPeriodAsync(command.CarId, It.IsAny<RentalPeriod>(), null)).ReturnsAsync(false);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<RentalDomainException>()
                  .WithMessage("*not available for the requested period*");

        _carRepoMock.Verify(x => x.GetByIdAsync(command.CarId), Times.Once);
        _rentalRepoMock.Verify(x => x.IsCarReservedInPeriodAsync(command.CarId, It.IsAny<RentalPeriod>(), null), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Car_NotFound()
    {
        var command = new RegisterRentalCommand(1, 999, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));

        _carRepoMock.Setup(r => r.GetByIdAsync(command.CarId)).ReturnsAsync((Car?)null);
        _rentalRepoMock.Setup(r => r.IsCarReservedInPeriodAsync(command.CarId, It.IsAny<RentalPeriod>(), null)).ReturnsAsync(false);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<RentalDomainException>()
                 .WithMessage("*not available for the requested period*");

        _carRepoMock.Verify(x => x.GetByIdAsync(command.CarId), Times.Once);
        _rentalRepoMock.Verify(x => x.IsCarReservedInPeriodAsync(command.CarId, It.IsAny<RentalPeriod>(), null), Times.Once);
    }
}
