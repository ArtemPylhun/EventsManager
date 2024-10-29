using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Events.Exceptions;
using Domain.Categories;
using Domain.Events;
using Domain.EventsTags;
using Domain.Locations;
using Domain.Tags;
using Domain.Users;
using MediatR;
using Optional;

namespace Application.Events.Commands;

public record UpdateEventCommand : IRequest<Result<Event, EventException>>
{
    public required Guid EventId { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required Guid OrganizerId { get; init; }
    public required Guid LocationId { get; init; }
    public required Guid CategoryId { get; init; }
    public required IReadOnlyList<Guid> TagsIds { get; init; }
}

public class UpdateEventCommandHandler(
    IEventRepository eventRepository,
    IEventQueries eventQueries,
    IUserQueries userQueries,
    ILocationQueries locationQueries,
    ICategoryQueries categoryQueries,
    IEventTagRepository eventTagRepository,
    IEventTagQueries eventTagQueries,
    ITagQueries tagQueries) : IRequestHandler<UpdateEventCommand, Result<Event, EventException>>
{
    public async Task<Result<Event, EventException>> Handle(
        UpdateEventCommand request,
        CancellationToken cancellationToken)
    {
        var existingEvent = await eventQueries.GetById(new EventId(request.EventId), cancellationToken);

        return await existingEvent.Match(
            async e =>
            {
                var existingOrganizer = await userQueries.GetById(new UserId(request.OrganizerId), cancellationToken);

                return await existingOrganizer.Match(
                    async eu =>
                    {
                        var existingLocation =
                            await locationQueries.GetById(new LocationId(request.LocationId), cancellationToken);

                        return await existingLocation.Match(
                            async el =>
                            {
                                var existingCategory = await categoryQueries.GetById(new CategoryId(request.CategoryId),
                                    cancellationToken);

                                return await existingCategory.Match(
                                    async ec =>
                                    {
                                        var entity = Event.New(
                                            new EventId(request.EventId),
                                            request.Title,
                                            request.Description,
                                            request.StartDate,
                                            request.EndDate,
                                            new UserId(request.OrganizerId),
                                            new LocationId(request.LocationId),
                                            new CategoryId(request.CategoryId));

                                        var duplicatedEvent = await CheckDuplicates(entity, cancellationToken);

                                        return await duplicatedEvent.Match(
                                            e => Task.FromResult<Result<Event, EventException>>
                                                (new EventAlreadyExistsException(e.Id)),
                                            async () => await UpdateEntity(entity,
                                                request.TagsIds.Select(ti => new TagId(ti)), cancellationToken));
                                    },
                                    () => Task.FromResult<Result<Event, EventException>>(
                                        new EventCategoryNotFoundException(EventId.Empty())));
                            },
                            () => Task.FromResult<Result<Event, EventException>>
                                (new EventLocationNotFoundException(EventId.Empty())));
                    },
                    () => Task.FromResult<Result<Event, EventException>>
                        (new EventOrganizerNotFoundException(EventId.Empty())));
            },
            () => Task.FromResult<Result<Event, EventException>>(
                new EventNotFoundException(new EventId(request.EventId))));
    }

    private async Task<Result<Event, EventException>> UpdateEntity(
        Event entity,
        IEnumerable<TagId> newTagsIds,
        CancellationToken cancellationToken)
    {
        try
        {
            var oldTags = await tagQueries.GetByEvent(entity.Id, cancellationToken);
            
            var tagsToRemove = new List<TagId>();
            var tagsToAdd = new List<TagId>();

            foreach (var newTagId in newTagsIds)
            {
                if (!oldTags.Any(ti => ti.Id == newTagId))
                {
                    tagsToAdd.Add(newTagId);
                }
            }
            foreach (var oldTag in oldTags)
            {
                if (!newTagsIds.Any(ti => ti == oldTag.Id))
                {
                    tagsToRemove.Add(oldTag.Id);
                }
            }

            foreach (var tagId in tagsToRemove)
            {
                var eventTag = await eventTagQueries.GetByEventAndTag(entity.Id, tagId, cancellationToken);

                await eventTag.Match(
                    async et => 
                    {
                        await eventTagRepository.Delete(et, cancellationToken);
                        return et;
                    },
                    () => Task.FromResult<EventTag?>(null)
                );
            }

            foreach (var tagId in tagsToAdd)
            {
                await eventTagRepository.Add(EventTag.New(EventTagId.New(), entity.Id, tagId), cancellationToken);
            }
            
            return await eventRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new EventUnknownException(EventId.Empty(), exception);
        }
    }

    private async Task<Option<Event>> CheckDuplicates(Event entity, CancellationToken cancellationToken)
    {
        var existingEvent = await eventQueries.SearchDuplicate(entity, cancellationToken);
        
        return existingEvent.Match(
            ee => ee.Id == entity.Id ? Option.None<Event>() : Option.Some(ee),
            Option.None<Event>);
    }
}