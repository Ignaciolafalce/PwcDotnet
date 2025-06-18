using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PwcDotnet.Domain.AggregatesModel.CustomerAggregate;

namespace PwcDotnet.Infrastructure.Data.EF.EntityConfigurations;

public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.Ignore(c => c.DomainEvents);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street).IsRequired().HasMaxLength(100);
            address.Property(a => a.City).IsRequired().HasMaxLength(100);
            address.Property(a => a.Country).IsRequired().HasMaxLength(100);
        });

        builder.Property(c => c.IdentityGuid)
            .HasMaxLength(200);

        builder.HasIndex(c => c.IdentityGuid);
    }
}
