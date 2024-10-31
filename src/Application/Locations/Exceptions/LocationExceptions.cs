using Domain.Locations;

namespace Application.Locations.Exceptions;

public class LocationException(LocationId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public LocationId LocationId { get; } = id;
}

public class LocationNotFoundException(LocationId id) : LocationException(id, $"Location under id {id} not found!");

public class LocationAlreadyExistsException(LocationId id) : LocationException(id, $"Such location already exists!");

public class LocationHasEventsException(LocationId id) : LocationException(id, $"Location has events and can's be deleted!");

public class LocationUnknownException(LocationId id, Exception innerException)
    : LocationException(id, $"Unknown exception for the location under id {id}!", innerException);