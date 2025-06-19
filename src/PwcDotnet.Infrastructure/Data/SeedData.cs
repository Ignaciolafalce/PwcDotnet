using Microsoft.AspNetCore.Identity;
using PwcDotnet.Application.Common.Auth;
using PwcDotnet.Domain.AggregatesModel.CarAggregate;
using PwcDotnet.Domain.AggregatesModel.CustomerAggregate;
using PwcDotnet.Domain.AggregatesModel.RentalAggregate;
using PwcDotnet.Infrastructure.Data.EF;

namespace PwcDotnet.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedDummyAsync(RentalDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        var identityUser = await userManager.FindByEmailAsync("admin@admin.com");

        if (identityUser is null)
        {
            throw new Exception("Admin user not found. Please ensure the admin user is created before seeding data.");
        }

        // Add some locations
        if (!context.Locations.Any())
        {
            context.Locations.AddRange(
                new Location("Buenos Aires"),
                new Location("Rosario"),
                new Location("Córdoba")
            );

            await context.SaveChangesAsync();
        }

        // Create a customer if it doesn't exist
        if (!context.Customers.Any())
        {
            var customer = new Customer("Juan Pérez", new Address("Av Siempreviva", "Springfield", "Argentina"), "juan@mail.com");
            customer.LinkToUser(identityUser!.Id);
            context.Customers.Add(customer);

            await context.SaveChangesAsync();
        }

        // Add some cars if they don't exist
        if (!context.Cars.Any())
        {
            var cars = new List<Car>() {
                new Car("Passat", "CC", new CarType("tipoJ"), 1),
                new Car("Ford", "Ka", new CarType("tipoH"), 1),
                new Car("Honda", "Civic", new CarType("Sedan"), 2)
            };

            cars.ForEach(car => car.LinkToUser(identityUser!.Id));
         
            context.Cars.AddRange(cars);

            await context.SaveChangesAsync();
        }

        // Add two services for the first car if it exists
        if (!context.Services.Any())
        {
            var existingCar = context.Cars.FirstOrDefault();
            if (existingCar is not null)
            {
                existingCar.ScheduleService(DateTime.UtcNow.AddDays(7));
                existingCar.ScheduleService(DateTime.UtcNow.AddDays(14));

                await context.SaveChangesAsync();
            }
        }

        if (!context.Rentals.Any())
        {
            var car = context.Cars.FirstOrDefault();
            var customer = context.Customers.FirstOrDefault();

            if (car is not null && customer is not null)
            {
                var rentalPeriod = new RentalPeriod(DateTime.UtcNow.Date, DateTime.UtcNow.AddDays(3));
                var rental = Rental.Create(customer.Id, car.Id, rentalPeriod);
                rental.LinkToUser(identityUser.Id);
                context.Rentals.Add(rental);

                await context.SaveChangesAsync();
            }
        }

        await context.SaveChangesAsync();
    }

    public static async Task SeedAdminUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        var roles = new[] { RoleConstants.Admin, RoleConstants.User, RoleConstants.Manager };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole(role));
            }
        }

        var adminEmail = "admin@admin.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            var user = new ApplicationUser
            {
                UserName = RoleConstants.Admin,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(user, roles);
            }
        }
    }
}
