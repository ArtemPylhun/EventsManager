using Domain.Locations;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface ILocationQueries
{
    Task<IReadOnlyList<Location>> GetAll(CancellationToken cancellationToken);
    Task<Option<Location>> SearchDuplicate(Location location, CancellationToken cancellationToken);
    Task<Option<Location>> GetById(LocationId id, CancellationToken cancellationToken);
}