using Domain.Events;

namespace Application.Events.Exceptions;

public class EventException(EventId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public EventId EventId { get; } = id;
}

public class EventNotFoundException(EventId id) : EventException(id, $"Event under id {id} not found!");

public class EventOrganizerNotFoundException(EventId id) : EventException(id, $"Event organizer not found!");

public class EventLocationNotFoundException(EventId id) : EventException(id, $"Event location not found!");

public class EventCategoryNotFoundException(EventId id) : EventException(id, $"Event category not found!");

public class EventAlreadyExistsException(EventId id) : EventException(id, $"Such event already exists!");
public class EventAlreadyFinishedException(EventId id) : EventException(id, $"Event is already finished!");

public class EventUnknownException(EventId id, Exception innerException)
    : EventException(id, $"Unknown exception for the event under id {id}!", innerException);