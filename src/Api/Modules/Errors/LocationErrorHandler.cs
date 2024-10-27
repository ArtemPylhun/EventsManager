using Application.Locations.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class LocationErrorHandler
{
    public static ObjectResult ToObjectResult(this LocationException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                LocationNotFoundException => StatusCodes.Status404NotFound,
                LocationAlreadyExistsException or LocationHasEventsException => StatusCodes.Status409Conflict,
                LocationUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Location error handler is not implemented")
            }
        };
    }
}