using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Events;
using Domain.Tags;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class TagRepository(ApplicationDbContext context) : ITagRepository, ITagQueries
{
    public async Task<IReadOnlyList<Tag>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Tags
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Tag>> GetByEvent(EventId eventId, CancellationToken cancellationToken)
    {
        var eventTags = await context.EventsTags
            .AsNoTracking()
            .Where(et => et.EventId == eventId)
            .Select(et => et.TagId)
            .ToListAsync(cancellationToken);
        
        return await context.Tags.AsNoTracking()
            .Where(t => eventTags.Contains(t.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Tag>> GetById(TagId id, CancellationToken cancellationToken)
    {
        var entity = await context.Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return entity == null ? Option.None<Tag>() : Option.Some(entity);
    }

    public async Task<Option<Tag>> SearchByTitle(string title, CancellationToken cancellationToken)
    {
        var entity = await context.Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Title == title, cancellationToken);
        
        return entity == null ? Option.None<Tag>() : Option.Some(entity);
    }

    public async Task<Tag> Add(Tag tag, CancellationToken cancellationToken)
    {
        await context.Tags.AddAsync(tag, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return tag;
    }

    public async Task<Tag> Update(Tag tag, CancellationToken cancellationToken)
    {
        context.Tags.Update(tag);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return tag;
    }

    public async Task<Tag> Delete(Tag tag, CancellationToken cancellationToken)
    {
        context.Tags.Remove(tag);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return tag;
    }
}