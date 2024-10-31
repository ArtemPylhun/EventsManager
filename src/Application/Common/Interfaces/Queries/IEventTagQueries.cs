using Domain.Events;
using Domain.EventsTags;
using Domain.Tags;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IEventTagQueries
{
    Task<IReadOnlyList<EventTag>> GetAll(CancellationToken cancellationToken);
    Task<Option<EventTag>> GetById(EventTagId id, CancellationToken cancellationToken);
    Task<Option<EventTag>> GetByEventAndTag(EventId eventId, TagId tagId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EventTag>> GetByEvent(EventId eventId, CancellationToken cancellationToken);
}