using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Locations.Exceptions;
using Domain.Locations;
using MediatR;
using Optional;

namespace Application.Locations.Commands;

public record UpdateLocationCommand : IRequest<Result<Location, LocationException>>
{
    public required Guid LocationId { get; init; }
    public required string Name { get; init; }
    public required string Address { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
    public required int Capacity { get; init; }
}

public class UpdateLocationCommandHandler(
    ILocationRepository locationRepository,
    ILocationQueries locationQueries) : IRequestHandler<UpdateLocationCommand, Result<Location, LocationException>>
{
    public async Task<Result<Location, LocationException>> Handle(
        UpdateLocationCommand request,
        CancellationToken cancellationToken)
    {
        var locationId = new LocationId(request.LocationId);
        var location = Location.New(locationId, request.Name, request.Address, request.City, request.Country, request.Capacity);
        var existingLocation = await locationQueries.GetById(locationId, cancellationToken);

        return await existingLocation.Match(
            async l =>
            {
                var duplicatedLocation = await CheckDuplicated(location, cancellationToken);

                return await duplicatedLocation.Match(
                    el => Task.FromResult<Result<Location, LocationException>>(new LocationAlreadyExistsException(el.Id)),
                    async () => await UpdateEntity(l, cancellationToken));
            },
            () => Task.FromResult<Result<Location, LocationException>>(new LocationNotFoundException(locationId)));
    }

    private async Task<Result<Location, LocationException>> UpdateEntity(
        Location location,
        CancellationToken cancellationToken)
    {
        try
        {
            location.UpdateDetails(location.Name, location.Address, location.City, location.Country, location.Capacity);

            return await locationRepository.Update(location, cancellationToken);
        }
        catch (Exception exception)
        {
            return new LocationUnknownException(location.Id, exception);
        }
    }

    private async Task<Option<Location>> CheckDuplicated(
        Location updatedEntity,
        CancellationToken cancellationToken)
    {
        var location = await locationQueries.SearchDuplicate(updatedEntity, cancellationToken);

        return location.Match(
            c => c.Id == updatedEntity.Id ? Option.None<Location>() : Option.Some(c),
            Option.None<Location>);
    }
}