using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PwcDotnet.Domain.AggregatesModel.CarAggregate;

namespace PwcDotnet.Infrastructure.Data.EF.EntityConfigurations;

public class LocationEntityTypeConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.Ignore(c => c.DomainEvents);

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
               .HasMaxLength(100)
               .IsRequired();
    }
}