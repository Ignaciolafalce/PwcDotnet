using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PwcDotnet.Domain.AggregatesModel.CarAggregate;

namespace PwcDotnet.Infrastructure.Data.EF.EntityConfigurations;

public class CarEntityTypeConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("Cars");

        builder.Ignore(c => c.DomainEvents);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Brand)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsOne(c => c.Type, type =>
        {
            type.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);
        });

        builder.HasMany(c => c.Services)
            .WithOne()
            .HasForeignKey("CarId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.IdentityGuid)
            .HasMaxLength(200);

        builder.HasIndex(c => c.IdentityGuid);
    }
}
