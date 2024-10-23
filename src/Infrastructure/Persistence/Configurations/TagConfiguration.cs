using Domain.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new TagId(x));
        builder.Property(x => x.Title).IsRequired().HasColumnType("varchar(255)");
        
        builder.HasMany(x => x.EventsTags)
            .WithOne(et => et.Tag)
            .HasForeignKey(et => et.TagId);
    }
}