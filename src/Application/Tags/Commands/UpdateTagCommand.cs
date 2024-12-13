using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Tags.Exceptions;
using Domain.Tags;
using MediatR;
using Optional;

namespace Application.Tags.Commands;

public record UpdateTagCommand : IRequest<Result<Tag, TagException>>
{
    public required Guid TagId { get; init; }
    public required string Title { get; init; }
}

public class UpdateTagCommandHandler(
    ITagRepository tagRepository,
    ITagQueries tagQueries) : IRequestHandler<UpdateTagCommand, Result<Tag, TagException>>
{
    public async Task<Result<Tag, TagException>> Handle(
        UpdateTagCommand request,
        CancellationToken cancellationToken)
    {
        var tagId = new TagId(request.TagId);
        var tag = await tagQueries.GetById(tagId, cancellationToken);

        return await tag.Match(
            async f =>
            {
                var existingTag = await CheckDuplicated(tagId, request.Title, cancellationToken);
                return await existingTag.Match(
                    f => Task.FromResult<Result<Tag, TagException>>(new TagAlreadyExistsException(f.Id)),
                    async () => await UpdateEntity(f, request.Title, cancellationToken));
            },
            () => Task.FromResult<Result<Tag, TagException>>(new TagNotFoundException(tagId)));
    }

    private async Task<Result<Tag, TagException>> UpdateEntity(
        Tag tag,
        string name,
        CancellationToken cancellationToken)
    {
        try
        {
            tag.UpdateDetails(name);

            return await tagRepository.Update(tag, cancellationToken);
        }
        catch (Exception exception)
        {
            return new TagUnknownException(tag.Id, exception);
        }
    }

    private async Task<Option<Tag>> CheckDuplicated(
        TagId tagId,
        string name,
        CancellationToken cancellationToken)
    {
        var category = await tagQueries.SearchByTitle(name, cancellationToken);

        return category.Match(
            c => c.Id == tagId ? Option.None<Tag>() : Option.Some(c),
            Option.None<Tag>);
    }
}