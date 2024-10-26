using Domain.Tags;

namespace Api.Dtos;

public record TagDto(
    Guid? Id,
    string Title)
{
    public static TagDto FromDomainModel(Tag tag)
        => new(
            Id: tag.Id.Value,
            Title: tag.Title);
}