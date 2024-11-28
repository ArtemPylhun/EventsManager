using Application.Users.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class UserErrorHandler
{
    public static ObjectResult ToObjectResult(this UserException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                UserNotFoundException or UserRoleNotFoundException or UserProfileNotFoundException
                    or UserEventNotFoundException or UserAttendanceNotFound => StatusCodes
                        .Status404NotFound,
                InvalidCredentialsException => StatusCodes.Status401Unauthorized,
                UserWithNameAlreadyExistsException or UserWithEmailAlreadyExistsException
                    or UserEventDateHasPassed or UserEventHasAlreadyStarted
                    or UserEventAlreadyRegistered => StatusCodes.Status409Conflict,
                UserUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("User error handler is not implemented")
            }
        };
    }
}