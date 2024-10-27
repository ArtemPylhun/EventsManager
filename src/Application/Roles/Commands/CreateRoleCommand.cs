using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Roles.Exceptions;
using Domain.Roles;
using MediatR;

namespace Application.Roles.Commands;

public record CreateRoleCommand : IRequest<Result<Role, RoleException>>
{
    public required string Title { get; init; }
}

public class CreateRoleCommandHandler(
    IRoleRepository roleRepository, IRoleQueries roleQueries) : IRequestHandler<CreateRoleCommand, Result<Role, RoleException>>
{
    public async Task<Result<Role, RoleException>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var existingRole = await roleQueries.SearchByTitle(request.Title, cancellationToken);


        return await existingRole.Match(
            r => Task.FromResult<Result<Role, RoleException>>(new RoleAlreadyExistsException(r.Id)),
            async () => await CreateEntity(request.Title, cancellationToken));
    }

    private async Task<Result<Role, RoleException>> CreateEntity(
        string title,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = Role.New(RoleId.New(),title);

            return await roleRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new RoleUnknownException(RoleId.Empty(), exception);
        }
    }
}