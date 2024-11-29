using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Users.Commands;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    [HttpPost("login")]
    public async Task<ActionResult> LoginUser([FromBody] UserLoginDto request, CancellationToken cancellationToken)
    {
        var input = new LoginUserCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult>(
            u => Ok(u),
            e => e.ToObjectResult());
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDto>> GetById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var entity = await userQueries.GetById(new UserId(userId), cancellationToken);

        return entity.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            () => NotFound());
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Create([FromBody] UserCreateDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateUserCommand
        {
            Email = request.Email,
            UserName = request.UserName,
            Password = request.Password,
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [HttpPut("update")]
    public async Task<ActionResult<UserDto>> Update([FromBody] UserUpdateDto request,
        CancellationToken cancellationToken)
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

        return result.Match<ActionResult<UserDto>>(
            user => UserDto.FromDomainModel(user),
            e => e.ToObjectResult());
    }

    [Authorize (Roles = "Admin")]
    [HttpPut("setRole")]
    public async Task<ActionResult<UserDto>> UpdateRole([FromBody] UpdateUserRoleDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdateUserRoleCommand
        {
            UserId = request.UserId,
            RoleId = request.RoleId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            user => UserDto.FromDomainModel(user),
            e => e.ToObjectResult());
    }

    [HttpDelete("delete/{userId:guid}")]
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

    [HttpPost("registerToEvent/{eventId:guid}")]
    public async Task<ActionResult<AttendanceRegisterDto>> RegisterToEvent([FromRoute] Guid eventId,
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        var input = new RegisterUserToEventCommand
        {
            UserId = userId,
            EventId = eventId
        };
        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<AttendanceRegisterDto>>(
            u => AttendanceRegisterDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }

    [HttpDelete("unregisterFromEvent/{eventId:guid}")]
    public async Task<ActionResult<AttendanceUnregisterDto>> UnregisterFromEvent([FromRoute] Guid eventId,
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        var input = new UnregisterUserFromEventCommand
        {
            UserId = userId,
            EventId = eventId
        };
        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<AttendanceUnregisterDto>>(
            u => AttendanceUnregisterDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
}