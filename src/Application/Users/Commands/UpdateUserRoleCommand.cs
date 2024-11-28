using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Roles;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record UpdateUserRoleCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    public required Guid RoleId { get; init; }
}

public class ChangeUserRoleCommandHandler(
    IUserRepository userRepository,
    IUserQueries userQueries,
    IRoleQueries roleQueries)
    : IRequestHandler<UpdateUserRoleCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(UpdateUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var roleId = new RoleId(request.RoleId);
        var existingRole = await roleQueries.GetById(roleId, cancellationToken);
        return await existingRole.Match(
            async r =>
            {
                var existingUser = await userQueries.GetById(userId, cancellationToken);
                return await existingUser.Match(
                    async u => await UpdateEntity(u, r, cancellationToken),
                    () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId)));
            }
            , () => Task.FromResult<Result<User, UserException>>(new UserRoleNotFoundException(UserId.Empty(), RoleId.Empty())));
    }

    private async Task<Result<User, UserException>> UpdateEntity(
        User user,
        Role role,
        CancellationToken cancellationToken)
    {
        try
        {
            user.UpdateRole(role.Id);
            return await userRepository.Update(user, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(user.Id, exception);
        }
    }
}