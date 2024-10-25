using Domain.Users;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfigurator: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new UserId(x));
        
        builder.Property(x => x.UserName).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Email).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.PasswordHash).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.RegisteredOn)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .HasConstraintName("fk_users_roles_id")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Profile)
            .WithOne()
            .HasForeignKey<User>(x => x.ProfileId)
            .HasConstraintName("fk_users_profile_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Attendances)
            .WithOne(uc => uc.User)
            .HasForeignKey(uc => uc.UserId);
    }
}