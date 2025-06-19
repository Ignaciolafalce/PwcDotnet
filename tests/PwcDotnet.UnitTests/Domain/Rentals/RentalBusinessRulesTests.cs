using FluentAssertions;
using PwcDotnet.Domain.AggregatesModel.RentalAggregate;
using PwcDotnet.Domain.Exceptions;

namespace PwcDotnet.UnitTests.Domain.Rentals;
public class RentalBusinessRulesTests
{
    [Fact]
    public void Create_Should_Create_Valid_Rental()
    {
        var period = new RentalPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(2));
        var rental = Rental.Create(1, 2, period);

        rental.CustomerId.Should().Be(1);
        rental.CarId.Should().Be(2);
        rental.Status.Should().Be(RentalStatus.Active);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    public void Create_Should_Throw_When_Invalid_Customer_Or_CarId(int customerId, int carId)
    {
        var period = new RentalPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(2));
        var act = () => Rental.Create(customerId, carId, period);
        act.Should().Throw<RentalDomainException>();
    }

    [Fact]
    public void Create_Should_Throw_When_Invalid_Period()
    {
        RentalPeriod? period = default!;
        var act = () => Rental.Create(1, 1, period);
        act.Should().Throw<RentalDomainException>();
    }

    [Fact]
    public void ChangePeriod_Should_Work_When_Valid()
    {
        var rental = Rental.Create(1, 1, new RentalPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(1)));
        var newPeriod = new RentalPeriod(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(3));

        rental.ChangePeriod(newPeriod);

        rental.Period.Should().Be(newPeriod);
    }

    [Fact]
    public void ChangePeriod_Should_Throw_Work_When_RentalStatus_NotActive()
    {
        var rental = Rental.Create(1, 1, new RentalPeriod(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2)));
        var newPeriod = new RentalPeriod(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(3));
        rental.Cancel();

        var act = () => rental.ChangePeriod(newPeriod);

        act.Should().Throw<RentalDomainException>();
    }

    [Fact]
    public void ChangePeriod_Should_Throw_Work_When_Invalid_NewPeriod()
    {
        var rental = Rental.Create(1, 1, new RentalPeriod(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2)));
        RentalPeriod? newPeriod = default!;

        var act = () => rental.ChangePeriod(newPeriod);

        act.Should().Throw<RentalDomainException>();
    }

    [Fact]
    public void ChangeCar_Should_Change_Car()
    {
        var rental = Rental.Create(1, 1, new RentalPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(1)));

        rental.ChangeCar(5);

        rental.CarId.Should().Be(5);
    }

    [Fact]
    public void ChangeCar_Should_Throw_Work_When_RentalStatus_Active()
    {
        var rental = Rental.Create(1, 1, new RentalPeriod(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2)));

        rental.Cancel();

        var act = () => rental.ChangeCar(2);

        act.Should().Throw<RentalDomainException>();
    }

    [Fact]
    public void Cancel_Should_Change_Status()
    {
        var rental = Rental.Create(1, 1, new RentalPeriod(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(3)));
        rental.Cancel();

        rental.Status.Should().Be(RentalStatus.Cancelled);
    }

    [Fact]
    public void Cancel_Should_Fail_When_Already_Started()
    {
        var rental = Rental.Create(1, 1, new RentalPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(2)));

        var act = () => rental.Cancel();

        act.Should().Throw<RentalDomainException>();
    }

    [Fact]
    public void LinkToUser_Should_Set_IdentityGuid()
    {
        var rental = Rental.Create(1, 1, new RentalPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(2)));
        rental.LinkToUser("guid-123");

        rental.IdentityGuid.Should().Be("guid-123");
    }
}
