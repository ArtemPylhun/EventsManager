using Domain.EventsTags;

namespace Application.Common.Interfaces.Repositories;

public interface IEventTagRepository
{
    Task<EventTag> Add(EventTag eventTag, CancellationToken cancellationToken);
    Task<EventTag> Update(EventTag eventTag, CancellationToken cancellationToken);
    Task<EventTag> Delete(EventTag eventTag, CancellationToken cancellationToken);
}