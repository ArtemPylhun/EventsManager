using Domain.Categories;
using Domain.Events;
using Domain.Locations;
using Domain.Tags;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IEventQueries
{
    Task<IReadOnlyList<Event>> GetAll(CancellationToken cancellationToken);
    Task<IReadOnlyList<Event>> GetByTags(IReadOnlyList<Tag> tags, CancellationToken cancellationToken);
    Task<IReadOnlyList<Event>> GetByCategory(CategoryId categoryId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Event>> GetByLocation(LocationId locationId, CancellationToken cancellationToken);
    Task<Option<Event>> GetById(EventId id, CancellationToken cancellationToken);
    Task<Option<Event>> SearchDuplicate(Event entity, CancellationToken cancellationToken);
    Task<Option<Event>> SearchByTitle(string title, CancellationToken cancellationToken);
}