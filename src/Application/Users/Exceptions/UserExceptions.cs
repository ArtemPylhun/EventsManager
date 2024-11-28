using Domain.Events;
using Domain.Profiles;
using Domain.Roles;
using Domain.Users;

namespace Application.Users.Exceptions;

public class UserException(UserId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public UserId UserId { get; } = id;
}

public class UserNotFoundException(UserId id) : UserException(id, $"User under id: {id} not found!");

public class UserWithNameAlreadyExistsException(UserId id) : UserException(id, $"User under such name already exists!");

public class UserWithEmailAlreadyExistsException(UserId id)
    : UserException(id, $"User under such email already exists!");

public class UserRoleNotFoundException(UserId id, RoleId roleId)
    : UserException(id, $"User's role under id: {roleId} not found!");

public class UserProfileNotFoundException(UserId id, ProfileId profileId)
    : UserException(id, $"User's profile under id: {profileId} not found!");

public class InvalidCredentialsException() : UserException(UserId.Empty(), $"Invalid credentials!");

public class UserUnknownException(UserId id, Exception innerException)
    : UserException(id, $"Unknown exception for the User under id: {id}!", innerException);

public class UserEventDateHasPassed(UserId id, EventId eventId)
    : UserException(id, $"Event with such id: {eventId} has already passed!");

public class UserEventHasAlreadyStarted(UserId id, EventId eventId)
    : UserException(id, $"Event with such id: {eventId} has already started!");

public class UserEventNotFoundException(UserId id, EventId eventId)
    : UserException(id, $"Event with such id: {eventId} was not found!");

public class UserEventAlreadyRegistered(UserId id, EventId eventId)
    : UserException(id, $"User has already registered to the event with such id: {eventId}!");

public class UserAttendanceNotFound(UserId id, EventId eventId)
    : UserException(id, $"User has not registered to the event with such id: {eventId}!");