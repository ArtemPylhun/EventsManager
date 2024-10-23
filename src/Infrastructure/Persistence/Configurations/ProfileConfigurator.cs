using Domain.Profiles;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProfileConfigurator : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new ProfileId(x));
        
        builder.Property(x => x.FullName).IsRequired().HasColumnType("varchar(100)");
        
        builder.Property(x => x.BirthDate)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property(x => x.Address).HasColumnType("varchar(100)");
        builder.Property(x => x.PhoneNumber).HasColumnType("varchar(100)");
    }
}