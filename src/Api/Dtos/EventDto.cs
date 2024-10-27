using Domain.Events;

namespace Api.Dtos;

public record EventCreateDto(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    Guid OrganizerId,
    Guid LocationId,
    Guid CategoryId,
    IReadOnlyList<Guid> TagsIds)
{
    
}

public record EventUpdateDto(
    Guid EventId,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    Guid OrganizerId,
    Guid LocationId,
    Guid CategoryId,
    IReadOnlyList<Guid> TagsIds)
{
    
}

public record EventDto(
    Guid Id,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    Guid OrganizerId,
    Guid LocationId,
    Guid CategoryId)
{
    public static EventDto FromDomainModel(Event entity)
        => new(
            entity.Id.Value,
            entity.Title,
            entity.Description,
            entity.StartDate,
            entity.EndDate,
            entity.OrganizerId.Value,
            entity.LocationId.Value,
            entity.CategoryId.Value);
}