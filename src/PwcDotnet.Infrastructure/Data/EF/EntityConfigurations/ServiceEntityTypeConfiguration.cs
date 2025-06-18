using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PwcDotnet.Domain.AggregatesModel.CarAggregate;

namespace PwcDotnet.Infrastructure.Data.EF.EntityConfigurations;

public class ServiceEntityTypeConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");

        builder.Ignore(s => s.DomainEvents);

        builder.HasKey(s => s.Id);

        builder.Property<DateTime>("Date")
            .IsRequired();

        builder.Property<int>("CarId")
            .IsRequired();

        builder.HasIndex("CarId");
    }
}
