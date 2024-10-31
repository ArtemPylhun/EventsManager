using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Locations.Exceptions;
using Domain.Locations;
using MediatR;

namespace Application.Locations.Commands;

public record DeleteLocationCommand : IRequest<Result<Location, LocationException>>
{
    public required Guid LocationId { get; init; }
}

public class DeleteLocationCommandHandler(
    ILocationRepository locationRepository,
    ILocationQueries locationQueries,
    IEventQueries eventQueries) : IRequestHandler<DeleteLocationCommand, Result<Location, LocationException>>
{
    public async Task<Result<Location, LocationException>> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
    {
        var locationId = new LocationId(request.LocationId);
        var location = await locationQueries.GetById(locationId, cancellationToken);

        return await location.Match(
            async l =>
            {
                var locationEvents = await eventQueries.GetByLocation(locationId, cancellationToken);
                if (locationEvents.Any())
                {
                    return new LocationHasEventsException(locationId);
                }
                return await DeleteEntity(l, cancellationToken);
            },
            () => Task.FromResult<Result<Location, LocationException>>(new LocationNotFoundException(locationId)));
    }

    private async Task<Result<Location, LocationException>> DeleteEntity(Location location,
        CancellationToken cancellationToken)
    {
        try
        {
            return await locationRepository.Delete(location, cancellationToken);
        }
        catch (Exception exception)
        {
            return new LocationUnknownException(location.Id, exception);
        }
    }
}