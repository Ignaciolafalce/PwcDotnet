using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PwcDotnet.Application.Commands.Auth;
using PwcDotnet.Application.Common.Auth;
using PwcDotnet.Application.Common.Auth.IdentityEntities;
using PwcDotnet.Application.DTOs;
using PwcDotnet.Domain.AggregatesModel.CarAggregate;
using PwcDotnet.Domain.AggregatesModel.CustomerAggregate;
using PwcDotnet.Domain.AggregatesModel.RentalAggregate;
using PwcDotnet.Infrastructure.Data.EF;
using PwcDotnet.WebAPI;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PwcDotnet.IntegrationTests.RentalApi;

public class RentalApiSetupFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<RentalDbContext>));

            if (descriptor is not null) services.Remove(descriptor);

            // Add in-memory database for tests
            services.AddDbContext<RentalDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestRentalDb");
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<RentalDbContext>();

            // Ensure database is created
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            SeedUsersAsync(scopedServices).GetAwaiter().GetResult();
        });
    }

    public async Task AuthenticateAdminAsync(HttpClient client)
    {
        var tokenCommand = new TokenCommand("admin@admin.com", "Admin123!");

        var response = await client.PostAsJsonAsync("/auth/login", tokenCommand);
        var result = await response.Content.ReadFromJsonAsync<TokenDto>();

        var token = result!.AccessToken.Trim().Replace("Bearer", ""); // token without Bearer word
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task SeedUsersAsync(IServiceProvider services)
    {
        var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var adminRole = new ApplicationRole { Name = RoleConstants.Admin };
        if (!await roleManager.RoleExistsAsync(adminRole.Name))
        {
            await roleManager.CreateAsync(adminRole);
        }

        var adminUser = new ApplicationUser
        {
            UserName = "admin@admin.com",
            Email = "admin@admin.com"
        };

        if (await userManager.FindByNameAsync(adminUser.UserName) is not null)
        {
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, RoleConstants.Admin);
        }
    }

    public async Task<List<Rental>> DefaultRentalSeedAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RentalDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // remove dummy data if any...
        dbContext.Customers.RemoveRange(dbContext.Customers);
        await dbContext.SaveChangesAsync();

        dbContext.Cars.RemoveRange(dbContext.Cars);
        await dbContext.SaveChangesAsync();

        dbContext.Rentals.RemoveRange(dbContext.Rentals);
        await dbContext.SaveChangesAsync();

        var identityUser = await userManager.FindByEmailAsync("admin@admin.com");

        if (identityUser is null)
        {
            throw new Exception("Admin user not found. Please ensure the admin user is created before seeding data.");
        }

        // Seed test data
        var customer = new Customer("Test Customer", new Address("Street 1", "City", "Country"), "test@mail.com");
        customer.LinkToUser(identityUser.Id);
        dbContext.Customers.Add(customer);

        var car = new Car("Make", "Model", new CarType("Type"), 1);
        car.LinkToUser(identityUser.Id);
        dbContext.Cars.Add(car);

        var period = new RentalPeriod(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(5));
        var rental = Rental.Create(customer.Id, car.Id, period);
        rental.LinkToUser(identityUser.Id);
        dbContext.Rentals.Add(rental);

        dbContext.SaveChanges();

        return dbContext.Rentals.ToList();
    }
}
