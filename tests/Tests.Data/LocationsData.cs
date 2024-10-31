using Domain.Locations;

namespace Tests.Data;

public class LocationsData
{
    private static int _minCapacity = 5;

    public static Location MainLocation => Location.New(
        LocationId.New(),
        "Main location name",
        "Main location address",
        "Main location city",
        "Main location country",
        _minCapacity);
    
    public static Location SecondaryLocation => Location.New(
        LocationId.New(),
        "Secondary location name",
        "Secondary location address",
        "Secondary location city",
        "Secondary location country",
        _minCapacity);
}