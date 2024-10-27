using Domain.Locations;

namespace Api.Dtos;

public record LocationDto(
    Guid? LocationId,
    string Name,
    string Address,
    string City,
    string Country,
    int Capacity)
{
    public static LocationDto FromDomainModel(Location location)
        => new (location.Id.Value,
            location.Name,
            location.Address,
            location.City,
            location.Country,
            location.Capacity);
}