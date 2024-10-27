using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Events.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EventId = Domain.Events.EventId;

namespace Api.Controllers;

[Route("events")]
[ApiController]
public class EventsController(ISender sender, IEventQueries eventQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EventDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await eventQueries.GetAll(cancellationToken);
        return entities.Select(EventDto.FromDomainModel).ToList();
    }
    
    [HttpGet("{eventId:guid}")]
    public async Task<ActionResult<EventDto>> GetById([FromRoute] Guid eventId, CancellationToken cancellationToken)
    {
        var entity = await eventQueries.GetById(new EventId(eventId), cancellationToken);
        return entity.Match<ActionResult<EventDto>>(
            e => EventDto.FromDomainModel(e),
            () => NotFound());
    }
    
    [HttpPost]
    public async Task<ActionResult<EventDto>> Create(
        [FromBody] EventCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateEventCommand
        {
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            OrganizerId = request.OrganizerId,
            LocationId = request.LocationId,
            CategoryId = request.CategoryId,
            TagsIds = request.TagsIds
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<EventDto>>(
            ev => EventDto.FromDomainModel(ev),
            e => e.ToObjectResult());
    }
    [HttpPut]
    public async Task<ActionResult<EventDto>> Update(
        [FromBody] EventUpdateDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateEventCommand
        {
            EventId = request.EventId,
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            OrganizerId = request.OrganizerId,
            LocationId = request.LocationId,
            CategoryId = request.CategoryId,
            TagsIds = request.TagsIds
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<EventDto>>(
            ev => EventDto.FromDomainModel(ev),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<EventDto>> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteEventCommand
        {
            EventId = id
        };
        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<EventDto>>(
            ev => EventDto.FromDomainModel(ev),
            e => e.ToObjectResult());
    }
}