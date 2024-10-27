using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Roles.Exceptions;
using Domain.Roles;
using MediatR;

namespace Application.Roles.Commands;

public record DeleteRoleCommand : IRequest<Result<Role, RoleException>>
{
    public required Guid RoleId { get; init; }
}

public class DeleteRoleCommandHandler(
    IRoleRepository roleRepository,
    IRoleQueries roleQueries
    ) : IRequestHandler<DeleteRoleCommand, Result<Role, RoleException>>
{
    public async Task<Result<Role, RoleException>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var roleId = new RoleId(request.RoleId);
        var existingRole = await roleQueries.GetById(roleId, cancellationToken);
        
        return await existingRole.Match(
            async role => await DeleteEntity(role, cancellationToken),
            () => Task.FromResult<Result<Role, RoleException>>(new RoleNotFoundException(roleId)));
    }
    
    private async Task<Result<Role, RoleException>> DeleteEntity(Role role, CancellationToken cancellationToken)
    {
        try
        {
            return await roleRepository.Delete(role, cancellationToken);
        }
        catch (Exception exception)
        {
            return new RoleUnknownException(role.Id, exception);
        }
    }
}