using Domain.Roles;

namespace Application.Common.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role> Add(Role course, CancellationToken cancellationToken);
    Task<Role> Update(Role course, CancellationToken cancellationToken);
    Task<Role> Delete(Role course, CancellationToken cancellationToken);

}