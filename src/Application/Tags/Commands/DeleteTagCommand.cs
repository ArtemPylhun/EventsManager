using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Tags.Exceptions;
using Domain.Tags;
using MediatR;

namespace Application.Tags.Commands;

public record DeleteTagCommand : IRequest<Result<Tag, TagException>>
{
    public required Guid TagId { get; init; }
}

public class DeleteTagCommandHandler(
    ITagRepository tagRepository,
    ITagQueries tagQueries
    ) : IRequestHandler<DeleteTagCommand, Result<Tag, TagException>>
{
    public async Task<Result<Tag, TagException>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tagId = new TagId(request.TagId);
        var existingTag = await tagQueries.GetById(tagId, cancellationToken);
        return await existingTag.Match(
            async tag => await DeleteEntity(tag, cancellationToken),
            () => Task.FromResult<Result<Tag, TagException>>(new TagNotFoundException(tagId)));
    }
    
    private async Task<Result<Tag, TagException>> DeleteEntity(Tag tag, CancellationToken cancellationToken)
    {
        try
        {
            return await tagRepository.Delete(tag, cancellationToken);
        }
        catch (Exception exception)
        {
            return new TagUnknownException(tag.Id, exception);
        }
    }
}