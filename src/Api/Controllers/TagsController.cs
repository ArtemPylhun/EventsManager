using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Tags.Commands;
using Domain.Tags;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("tags")]
[ApiController]
public class TagsController(ISender sender, ITagQueries facultyQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TagDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await facultyQueries.GetAll(cancellationToken);

        return entities.Select(TagDto.FromDomainModel).ToList();
    }

    [HttpGet("{tagsId:guid}")]
    public async Task<ActionResult<TagDto>> GetById([FromRoute] Guid tagsId, CancellationToken cancellationToken)
    {
        var entity = await facultyQueries.GetById(new TagId(tagsId), cancellationToken);
        return entity.Match<ActionResult<TagDto>>(
            f => TagDto.FromDomainModel(f),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> Create(
        [FromBody] TagDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateTagCommand
        {
            Title = request.Title
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<TagDto>>(
            f => TagDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }

    [HttpPut]
    public async Task<ActionResult<TagDto>> Update(
        [FromBody] TagDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateTagCommand
        {
            TagId = request.Id.Value,
            Title = request.Title
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<TagDto>>(
            f => TagDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<TagDto>> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteTagCommand
        {
            TagId = id
        };

        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<TagDto>>(
            f => TagDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
}