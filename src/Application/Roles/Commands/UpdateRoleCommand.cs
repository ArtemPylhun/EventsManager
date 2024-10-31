using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Roles.Exceptions;
using Domain.Roles;
using MediatR;
using Optional;

namespace Application.Roles.Commands;

public record UpdateRoleCommand : IRequest<Result<Role, RoleException>>
{
    public required Guid RoleId { get; init; }
    public required string Title { get; init; }
}

public class UpdateRoleCommandHandler(
    IRoleRepository roleRepository,
    IRoleQueries roleQueries) : IRequestHandler<UpdateRoleCommand, Result<Role, RoleException>>
{
    public async Task<Result<Role, RoleException>> Handle(
        UpdateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var roleId = new RoleId(request.RoleId);
        var role = await roleQueries.GetById(roleId, cancellationToken);

        return await role.Match(
            async r =>
            {
                var existingRole = await roleQueries.SearchByTitle(request.Title, cancellationToken);

                return await existingRole.Match(
                    er => Task.FromResult<Result<Role, RoleException>>(new RoleAlreadyExistsException(er.Id)),
                    async () => await UpdateEntity(r, request.Title, cancellationToken));
            },
            () => Task.FromResult<Result<Role, RoleException>>(new RoleNotFoundException(roleId)));
    }

    private async Task<Result<Role, RoleException>> UpdateEntity(
        Role role,
        string name,
        CancellationToken cancellationToken)
    {
        try
        {
            role.UpdateDetails(name);

            return await roleRepository.Update(role, cancellationToken);
        }
        catch (Exception exception)
        {
            return new RoleUnknownException(role.Id, exception);
        }
    }
}