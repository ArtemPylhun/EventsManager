using Domain.Locations;

namespace Application.Common.Interfaces.Repositories;

public interface ILocationRepository
{
    Task<Location> Add(Location location, CancellationToken cancellationToken);
    Task<Location> Update(Location location, CancellationToken cancellationToken);
    Task<Location> Delete(Location location, CancellationToken cancellationToken);

}