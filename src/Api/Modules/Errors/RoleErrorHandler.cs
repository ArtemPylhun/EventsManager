using Application.Roles.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class RoleErrorHandler
{
    public static ObjectResult ToObjectResult(this RoleException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                RoleNotFoundException => StatusCodes.Status404NotFound,
                RoleAlreadyExistsException or RoleHaveUsersException => StatusCodes.Status409Conflict,
                RoleUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Faculty error handler does not implemented")
            }
        };
    }
}