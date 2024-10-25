using Domain.Events;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IEventRepository
{
    Task<Event> Add(Event entity, CancellationToken cancellationToken);
    Task<Event> Update(Event entity, CancellationToken cancellationToken);
    Task<Event> Delete(Event entity, CancellationToken cancellationToken);
}