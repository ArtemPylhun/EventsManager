using Application.Events.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class EventErrorHandler
{
    public static ObjectResult ToObjectResult(this EventException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                EventNotFoundException or
                    EventOrganizerNotFoundException or 
                        EventLocationNotFoundException or
                            EventCategoryNotFoundException => StatusCodes.Status404NotFound,
                EventAlreadyExistsException or
                    EventAlreadyFinishedException=> StatusCodes.Status409Conflict,
                EventUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Event error handler is not implemented")
            }
        };
    }
}