using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PwcDotnet.Domain.AggregatesModel.RentalAggregate;

namespace PwcDotnet.Infrastructure.Data.EF.EntityConfigurations;

public class RentalEntityTypeConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.ToTable("Rentals");

        builder.Ignore(r => r.DomainEvents);

        builder.HasKey(r => r.Id);

        builder.Property(r => r.CustomerId).IsRequired();
        builder.Property(r => r.CarId).IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.OwnsOne(r => r.Period, period =>
        {
            period.Property(p => p.Start).HasColumnName("StartDate").IsRequired();
            period.Property(p => p.End).HasColumnName("EndDate").IsRequired();
        });

        builder.HasOne(r => r.Car)
            .WithMany()
            .HasForeignKey(r => r.CarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(r => r.IdentityGuid)
            .HasMaxLength(200);

        builder.HasIndex(r => r.IdentityGuid);
    }
}
