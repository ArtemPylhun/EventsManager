using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Events;
using Domain.EventsTags;
using Domain.Tags;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class EventTagRepository(ApplicationDbContext context) : IEventTagRepository, IEventTagQueries
{
    public async Task<IReadOnlyList<EventTag>> GetAll(CancellationToken cancellationToken)
    {
        return await context.EventsTags
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<EventTag>> GetById(EventTagId id, CancellationToken cancellationToken)
    {
        var entity = await context.EventsTags
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return entity == null ? Option.None<EventTag>() : Option.Some(entity);
    }

    public async Task<Option<EventTag>> GetByEventAndTag(EventId eventId, TagId tagId, CancellationToken cancellationToken)
    {
        var entity = await context.EventsTags
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EventId == eventId && x.TagId == tagId, cancellationToken);
        
        return entity == null ? Option.None<EventTag>() : Option.Some(entity);
    }

    public async Task<IReadOnlyList<EventTag>> GetByEvent(EventId eventId, CancellationToken cancellationToken)
    {
        return await context.EventsTags
            .AsNoTracking()
            .Where(x => x.EventId == eventId)
            .ToListAsync(cancellationToken);
    }

    public async Task<EventTag> Add(EventTag eventTag, CancellationToken cancellationToken)
    {
        await context.EventsTags.AddAsync(eventTag, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return eventTag;
    }

    public async Task<EventTag> Update(EventTag eventTag, CancellationToken cancellationToken)
    {
        context.EventsTags.Update(eventTag);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return eventTag;
    }

    public async Task<EventTag> Delete(EventTag eventTag, CancellationToken cancellationToken)
    {
        context.EventsTags.Remove(eventTag);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return eventTag;
    }

}