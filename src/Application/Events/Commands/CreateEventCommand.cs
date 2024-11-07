using System.Text.RegularExpressions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Events.Exceptions;
using Domain.Categories;
using Domain.Events;
using Domain.EventsTags;
using Domain.Locations;
using Domain.Tags;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Events.Commands;

public record CreateEventCommand : IRequest<Result<Event, EventException>>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required Guid OrganizerId { get; init; }
    public required Guid LocationId { get; init; }
    public required Guid CategoryId { get; init; }
    public required IReadOnlyList<Guid> TagsIds { get; init; }
    public required IFormFile? Image { get; init; }
}

public class CreateEventCommandHandler(
    IEventRepository eventRepository,
    IEventQueries eventQueries,
    IUserQueries userQueries,
    ILocationQueries locationQueries,
    ICategoryQueries categoryQueries,
    IEventTagRepository eventTagRepository,
    IFileStorageService fileStorageService) : IRequestHandler<CreateEventCommand, Result<Event, EventException>>
{
    public async Task<Result<Event, EventException>> Handle(
        CreateEventCommand request,
        CancellationToken cancellationToken)
    {
        var existingOrganizer = await userQueries.GetById(new UserId(request.OrganizerId), cancellationToken);

        return await existingOrganizer.Match(
            async eu =>
            {
                var existingLocation = await locationQueries.GetById(new LocationId(request.LocationId), cancellationToken);
                
                return await existingLocation.Match(
                    async el =>
                    {
                        var existingCategory = await categoryQueries.GetById(new CategoryId(request.CategoryId), cancellationToken);

                        return await existingCategory.Match(
                            async ec =>
                            {
                                var entity = Event.New(EventId.New(), 
                                    request.Title,
                                    request.Description,
                                    request.StartDate,
                                    request.EndDate,
                                    null,
                                    new UserId(request.OrganizerId),
                                    new LocationId(request.LocationId),
                                    new CategoryId(request.CategoryId));
        
                                var existingEvent = await eventQueries.SearchDuplicate(entity, cancellationToken);

                                return await existingEvent.Match(
                                    e => Task.FromResult<Result<Event, EventException>>
                                        (new EventAlreadyExistsException(e.Id)),
                                    async () => await CreateEntity(entity, request.Image, request.TagsIds, cancellationToken));
                            },
                            () => Task.FromResult<Result<Event, EventException>>(
                                new EventCategoryNotFoundException(EventId.Empty())));
                    },
                    () => Task.FromResult<Result<Event, EventException>>
                        (new EventLocationNotFoundException(EventId.Empty())));
            },
            () => Task.FromResult<Result<Event, EventException>>
                (new EventOrganizerNotFoundException(EventId.Empty())));
    }

    private async Task<Result<Event, EventException>> CreateEntity(
        Event entity,
        IFormFile image,
        IReadOnlyList<Guid> tags,
        CancellationToken cancellationToken)
    {
        try
        {
            if (image != null)
            {
                string? imageUrl = null;
                imageUrl = await fileStorageService.SaveFileAsync(image, "events", entity.Id.Value, cancellationToken);

                entity.SetImageUrl(imageUrl);
            }
            
            var result = await eventRepository.Add(entity, cancellationToken);
            
            foreach (var tag in tags)
            {
                await eventTagRepository.Add(EventTag.New(EventTagId.New(), entity.Id, new TagId(tag)), cancellationToken);
            }

            return result;
        }
        catch (Exception exception)
        {
            return new EventUnknownException(EventId.Empty(), exception);
        }
    }
}