using Domain.Roles;

namespace Application.Roles.Exceptions;

public class RoleException(RoleId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public RoleId RoleId { get; } = id;
}

public class RoleNotFoundException(RoleId id) : RoleException(id, $"Role under id: {id} not found!");

public class RoleAlreadyExistsException(RoleId id) : RoleException(id, $"Role already exists: {id}!");

public class RoleHaveUsersException(RoleId id) : RoleException(id, $"Role under id: {id} has users!"); //TODO: Implement in role deletion

public class RoleUnknownException(RoleId id, Exception innerException)
    : RoleException(id, $"Unknown exception for the Role under id: {id}!", innerException);

