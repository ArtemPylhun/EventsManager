using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Tags.Exceptions;
using Domain.Tags;
using MediatR;

namespace Application.Tags.Commands;

public record CreateTagCommand : IRequest<Result<Tag, TagException>>
{
    public required string Title { get; init; }
}

public class CreateTagCommandHandler(
    ITagRepository tagRepository, ITagQueries tagQueries) : IRequestHandler<CreateTagCommand, Result<Tag, TagException>>
{
    public async Task<Result<Tag, TagException>> Handle(
        CreateTagCommand request,
        CancellationToken cancellationToken)
    {
        var existingTag = await tagQueries.SearchByTitle(request.Title, cancellationToken);

        return await existingTag.Match(
            f => Task.FromResult<Result<Tag, TagException>>(new TagAlreadyExistsException(f.Id)),
            async () => await CreateEntity(request.Title, cancellationToken));
    }

    private async Task<Result<Tag, TagException>> CreateEntity(
        string title,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = Tag.New(TagId.New(),title);

            return await tagRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new TagUnknownException(TagId.Empty(), exception);
        }
    }
}