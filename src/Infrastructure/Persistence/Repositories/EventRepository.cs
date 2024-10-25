using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Events;
using Domain.Tags;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class EventRepository(ApplicationDbContext context) : IEventRepository, IEventQueries
{
    public async Task<IReadOnlyList<Event>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Events
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetByTags(IReadOnlyList<Tag> tags, CancellationToken cancellationToken)
    {
        var eventTags = await context.Events
            .AsNoTracking()
            .Include(e => e.EventsTags)
            .Where(e => tags.All(tag => e.EventsTags.Any(et => et.TagId == tag.Id)))
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return eventTags;
    }
    
    public async Task<Option<Event>> GetById(EventId id, CancellationToken cancellationToken)
    {
        var entity = await context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return entity == null ? Option.None<Event>() : Option.Some(entity);
    }

    public async Task<Option<Event>> SearchByTitle(string title, CancellationToken cancellationToken)
    {
        var entity = await context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Title == title, cancellationToken);
        
        return entity == null ? Option.None<Event>() : Option.Some(entity);
    }

    public async Task<Event> Add(Event entity, CancellationToken cancellationToken)
    {
        await context.Events.AddAsync(entity, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return entity;
    }

    public async Task<Event> Update(Event entity, CancellationToken cancellationToken)
    {
        context.Events.Update(entity);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return entity;
    }

    public async Task<Event> Delete(Event entity, CancellationToken cancellationToken)
    {
        context.Events.Remove(entity);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return entity;
    }
}