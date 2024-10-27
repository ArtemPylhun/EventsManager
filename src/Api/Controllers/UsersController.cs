using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Commands;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("users")]
[ApiController]
public class UsersController(ISender sender, IUserQueries userQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await userQueries.GetAll(cancellationToken);

        return entities.Select(UserDto.FromDomainModel).ToList();
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDto>> GetById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var entity = await userQueries.GetById(new UserId(userId), cancellationToken);

        return entity.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] UserCreateDto request, CancellationToken cancellationToken)
    {
        var input = new CreateUserCommand
        {
            Email = request.Email,
            UserName = request.UserName,
            Password = request.Password,
            RoleId = request.RoleId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [HttpPut]
    public async Task<ActionResult<UserUpdateDto>> Update([FromBody] UserUpdateDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateUserCommand
        {
            UserId = request.Id,
            UserName = request.UserName,
            Password = request.Password,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            BirthDate = request.BirthDate            
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserUpdateDto>>(
            user => UserUpdateDto.FromUserUpdateDomainModel(user),
            e => e.ToObjectResult());
    }

    [HttpDelete("{userId:guid}")]
    public async Task<ActionResult<UserDto>> Delete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var input = new DeleteUserCommand
        {
            UserId = userId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
}