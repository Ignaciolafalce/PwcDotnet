using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PwcDotnet.Domain.AggregatesModel.CarAggregate;
using PwcDotnet.Domain.AggregatesModel.CustomerAggregate;
using PwcDotnet.Domain.AggregatesModel.RentalAggregate;
using PwcDotnet.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PwcDotnet.Infrastructure.Data.EF;


public class RentalDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>, IUnitOfWork
{
    private readonly IMediator _mediator;

    public RentalDbContext(DbContextOptions<RentalDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Location> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("Rental");

        // Apply configurations from the assembly :)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentalDbContext).Assembly);
        modelBuilder.Ignore<DomainEvent>();
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        var _ = await base.SaveChangesAsync(cancellationToken);

        await _mediator.DispatchDomainEventsAsync(this);

        return true;
    }
}