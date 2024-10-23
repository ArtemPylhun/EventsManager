using Domain.Events;
using Domain.EventsTags;
using Domain.Tags;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new EventId(x));
        builder.Property(x => x.Title).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Description).IsRequired().HasColumnType("varchar(1000)");

        builder.Property(x => x.StartDate)
            .HasConversion(new DateTimeUtcConverter());
        builder.Property(x => x.EndDate)
            .HasConversion(new DateTimeUtcConverter());

        builder.HasOne(x => x.Organizer)
            .WithMany()
            .HasForeignKey(x => x.OrganizerId)
            .HasConstraintName("fk_events_users_id")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey(x => x.LocationId)
            .HasConstraintName("fk_events_locations_id")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .HasConstraintName("fk_events_categories_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.EventsTags)
            .WithOne(et => et.Event)
            .HasForeignKey(et => et.EventId);
    }
}