﻿using Domain.EventsTags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class EventTagConfiguration : IEntityTypeConfiguration<EventTag>
{
    public void Configure(EntityTypeBuilder<EventTag> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new EventTagId(x));

        builder.HasOne(x => x.Event)
            .WithMany(x => x.EventsTags)
            .HasForeignKey(x => x.EventId)
            .HasConstraintName("fk_events_tags_events_id")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Tag)
            .WithMany(x => x.EventsTags)
            .HasForeignKey(x => x.TagId)
            .HasConstraintName("fk_events_tags_tags_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}