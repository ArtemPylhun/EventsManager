using Domain.Events;
using Domain.Tags;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface ITagQueries
{
    Task<IReadOnlyList<Tag>> GetAll(CancellationToken cancellationToken);
    Task<IReadOnlyList<Tag>> GetByEvent(EventId eventId, CancellationToken cancellationToken);
    Task<Option<Tag>> GetById(TagId id, CancellationToken cancellationToken);
    Task<Option<Tag>> SearchByName(string name, CancellationToken cancellationToken);
}