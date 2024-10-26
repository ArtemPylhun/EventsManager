using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Roles.Commands;
using Domain.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("roles")]
[ApiController]
public class RolesController(ISender sender, IRoleQueries facultyQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await facultyQueries.GetAll(cancellationToken);

        return entities.Select(RoleDto.FromDomainModel).ToList();
    }
    
    [HttpGet("{roleId:guid}")]
    public async Task<ActionResult<RoleDto>> GetById([FromRoute] Guid roleId, CancellationToken cancellationToken)
    {
        var entity = await facultyQueries.GetById(new RoleId(roleId), cancellationToken);
        return entity.Match<ActionResult<RoleDto>>(
            f => RoleDto.FromDomainModel(f),
            () => NotFound());
    }
    
    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create(
        [FromBody] RoleDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateRoleCommand
        {
            Title = request.Title
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<RoleDto>>(
            f => RoleDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }

    [HttpPut]
    public async Task<ActionResult<RoleDto>> Update(
        [FromBody] RoleDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateRoleCommand
        {
            RoleId = request.Id.Value,
            Title = request.Title
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<RoleDto>>(
            f => RoleDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RoleDto>> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteRoleCommand
        {
            RoleId = id
        };

        var result = await sender.Send(input, cancellationToken);
        return result.Match<ActionResult<RoleDto>>(
            f => RoleDto.FromDomainModel(f),
            e => e.ToObjectResult());
    }
}