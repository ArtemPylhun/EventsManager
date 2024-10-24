using Domain.Locations;

namespace Application.Common.Interfaces.Repositories;

public interface ILocationRepository
{
    Task<Location> Add(Location course, CancellationToken cancellationToken);
    Task<Location> Update(Location course, CancellationToken cancellationToken);
    Task<Location> Delete(Location course, CancellationToken cancellationToken);

}