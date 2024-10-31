using Domain.Attendances;
using Domain.Attendencies;
using Domain.Roles;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AttendanceConfigurator : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new AttendanceId(x));

        builder.Property(x => x.RegistrationDate)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.HasOne(a => a.User)
            .WithMany(u => u.Attendances)
            .HasForeignKey(a => a.UserId)
            .HasConstraintName("fk_attendances_users_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ue => ue.Event)
            .WithMany(e => e.Attendances)
            .HasForeignKey(e => e.EventId)
            .HasConstraintName("fk_attendances_events_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
