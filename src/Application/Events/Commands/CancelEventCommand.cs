using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Events.Exceptions;
using Domain.Events;
using MediatR;

namespace Application.Events.Commands;

public record CancelEventCommand : IRequest<Result<Event, EventException>>
{
    public required Guid EventId { get; init; }
}

public class CancelEventCommandHandler(
    IEventRepository eventRepository,
    IEventQueries eventQueries,
    IAttendanceQueries attendanceQueries,
    IAttendanceRepository attendanceRepository) : IRequestHandler<CancelEventCommand, Result<Event, EventException>>
{
    public async Task<Result<Event, EventException>> Handle(
        CancelEventCommand request,
        CancellationToken cancellationToken)
    {
        var eventId = new EventId(request.EventId);
        var existingEvent = await eventQueries.GetById(eventId, cancellationToken);

        return await existingEvent.Match(
            async e => await DeleteEntity(e, cancellationToken), 
            () => Task.FromResult<Result<Event, EventException>>(new EventNotFoundException(eventId)));
    }

    private async Task<Result<Event, EventException>> DeleteEntity(
        Event entity,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.Finish();
            
            var attendances = await attendanceQueries.SearchByEventId(entity.Id, cancellationToken);
            foreach (var attendance in attendances)
            {
                await attendanceRepository.Delete(attendance, cancellationToken);
            }
            
            return await eventRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new EventUnknownException(EventId.Empty(), exception);
        }
    }
}