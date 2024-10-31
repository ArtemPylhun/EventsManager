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