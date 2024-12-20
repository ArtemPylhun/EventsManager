﻿using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Events.Exceptions;
using Domain.Events;
using MediatR;

namespace Application.Events.Commands;

public record DeleteEventCommand : IRequest<Result<Event, EventException>>
{
    public required Guid EventId { get; init; }
}

public class DeleteEventCommandHandler(
    IEventRepository eventRepository,
    IEventQueries eventQueries,
    IEventTagRepository eventTagRepository,
    IEventTagQueries eventTagQueries) : IRequestHandler<DeleteEventCommand, Result<Event, EventException>>
{
    public async Task<Result<Event, EventException>> Handle(
        DeleteEventCommand request,
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
            return await eventRepository.Delete(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new EventUnknownException(EventId.Empty(), exception);
        }
    }
}