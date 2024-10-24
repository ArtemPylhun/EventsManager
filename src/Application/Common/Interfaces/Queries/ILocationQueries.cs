using Domain.Locations;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface ILocationQueries
{
    Task<IReadOnlyList<Location>> GetAll(CancellationToken cancellationToken);
    //Task<Option<Location>> SearchByName(string name, CancellationToken cancellationToken);
    Task<Option<Location>> GetById(LocationId id, CancellationToken cancellationToken);
}