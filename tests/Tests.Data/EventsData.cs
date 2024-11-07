using Domain.Categories;
using Domain.Events;
using Domain.Locations;
using Domain.Users;

namespace Tests.Data;

public class EventsData
{
    public static Event MainEvent(UserId organizerId, LocationId locationId, CategoryId categoryId)
        => Event.New(EventId.New(),
            "Main event title",
            "Main event description",
            DateTime.UtcNow + TimeSpan.FromHours(1),
            DateTime.UtcNow + TimeSpan.FromHours(10),
            null,
            organizerId,
            locationId,
            categoryId);
    
    public static Event SecondaryEvent(UserId organizerId, LocationId locationId, CategoryId categoryId)
        => Event.New(EventId.New(),
            "Secondary event title",
            "Secondary event description",
            DateTime.UtcNow + TimeSpan.FromHours(1),
            DateTime.UtcNow + TimeSpan.FromHours(10),
            null,
            organizerId,
            locationId,
            categoryId);
}