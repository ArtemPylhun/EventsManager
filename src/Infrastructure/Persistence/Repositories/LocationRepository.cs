using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class LocationRepository(ApplicationDbContext context): ILocationRepository, ILocationQueries
{
    public async Task<IReadOnlyList<Location>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Locations
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /*public async Task<Option<Location>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return entity == null ? Option.None<Location>() : Option.Some(entity);
    }*/

    public async Task<Option<Location>> GetById(LocationId id, CancellationToken cancellationToken)
    {
        var entity = await context.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Location>() : Option.Some(entity);
    }

    public async Task<Location> Add(Location location, CancellationToken cancellationToken)
    {
        await context.Locations.AddAsync(location, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return location;
    }

    public async Task<Location> Update(Location location, CancellationToken cancellationToken)
    {
        context.Locations.Update(location);

        await context.SaveChangesAsync(cancellationToken);

        return location;
    }
    
    public async Task<Location> Delete(Location location, CancellationToken cancellationToken)
    {
        context.Locations.Remove(location);

        await context.SaveChangesAsync(cancellationToken);

        return location;
    }
}