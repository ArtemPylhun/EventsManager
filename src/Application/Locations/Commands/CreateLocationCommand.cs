using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Locations.Exceptions;
using Domain.Locations;
using MediatR;

namespace Application.Locations.Commands;

public record CreateLocationCommand : IRequest<Result<Location, LocationException>>
{
    public required string Name { get; init; }
    public required string Address { get; init; }
    public required string City { get; init; }
    public required string Country { get; init; }
    public required int Capacity { get; init; }
}

public class CreateLocationCommandHandler(
    ILocationRepository locationRepository,
    ILocationQueries locationQueries) : IRequestHandler<CreateLocationCommand, Result<Location, LocationException>>
{
    public async Task<Result<Location, LocationException>> Handle(
        CreateLocationCommand request,
        CancellationToken cancellationToken)
    {
        var location = Location.New(LocationId.New(), request.Name, request.Address, request.City, request.Country, request.Capacity);
        var existingLocation = await locationQueries.SearchDuplicate(location, cancellationToken);

        return await existingLocation.Match(
            l => Task.FromResult<Result<Location, LocationException>>(new LocationAlreadyExistsException(l.Id)),
            async () => await CreateEntity(location, cancellationToken));
    }

    private async Task<Result<Location, LocationException>> CreateEntity(
        Location entity,
        CancellationToken cancellationToken)
    {
        try
        {
            return await locationRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new LocationUnknownException(LocationId.Empty(), exception);
        }
    }
}