using Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new LocationId(x));
        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Address).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.City).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Country).IsRequired().HasColumnType("varchar(255)");
    }
}