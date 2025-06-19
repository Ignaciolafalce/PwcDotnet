using FluentAssertions;
using PwcDotnet.Application.Commands;
using PwcDotnet.Application.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace PwcDotnet.IntegrationTests.RentalApi;

public class RentalEndpointTests : IClassFixture<RentalApiSetupFixture>
{
    private readonly RentalApiSetupFixture _fixture;
    private readonly HttpClient _client;

    public RentalEndpointTests(RentalApiSetupFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
        _fixture.AuthenticateAdminAsync(_client).Wait();
    }

    [Fact]
    public async Task GetAllRentals_ReturnsOkAndList()
    {
        // Arrange 
        var rentalsInDb = await _fixture.DefaultRentalSeedAsync();

        // Act
        var response = await _client.GetAsync("/rentals");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var rentals = await response.Content.ReadFromJsonAsync<RentalDto[]>();
        rentals.Should().NotBeNull();
        rentals.Length.Should().BeGreaterThan(0);
        rentals.First().Id.Should().BeGreaterThan(0);
        rentals.First().CustomerName.Should().Be(rentalsInDb.First().Customer.FullName);
        rentals.First().CarModel.Should().Be(rentalsInDb.First().Car.Model);
        rentals.First().CarType.Should().Be(rentalsInDb.First().Car.Type.Name);
        rentals.First().LocationId.Should().Be(rentalsInDb.First().Car.LocationId);
    }

    // this is the important test required! we can add more like unathorized, bad request, etc...
    [Fact]
    public async Task PostRegisterRental_ReturnsOkWithId()
    {
        // Arrange: use seeded customer and car
        var rentalsInDb = await _fixture.DefaultRentalSeedAsync();

        var carId = rentalsInDb.First().CarId;
        var customerId = rentalsInDb.First().CustomerId;

        var command = new RegisterRentalCommand(
            CustomerId: customerId,
            CarId: carId,
            StartDate: DateTime.UtcNow.AddDays(100),
            EndDate: DateTime.UtcNow.AddDays(120)
        );

        // Act
        var response = await _client.PostAsJsonAsync("/rentals/register", command);
        var registeredRental = await response.Content.ReadFromJsonAsync<RentalDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        registeredRental!.Id.Should().BeGreaterThan(0);
        registeredRental.CustomerId.Should().Be(command.CustomerId);
        registeredRental.CarId.Should().Be(command.CarId);
        registeredRental.StartDate.Should().Be(command.StartDate);
        registeredRental.EndDate.Should().Be(command.EndDate);
    }
}
