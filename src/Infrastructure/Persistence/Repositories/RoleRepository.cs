using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Roles;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class RoleRepository(ApplicationDbContext context) : IRoleRepository, IRoleQueries
{
    public async Task<IReadOnlyList<Role>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Roles
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    
    public async Task<int> GetRoleUsersCount(RoleId roleId, CancellationToken cancellationToken)
    {
        return await context.Users
            .AsNoTracking()
            .CountAsync(u => u.RoleId == roleId, cancellationToken);
    }

    public async Task<Option<Role>> SearchByTitle(string title, CancellationToken cancellationToken)
    {
        var entity = await context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Title == title, cancellationToken);

        return entity == null ? Option.None<Role>() : Option.Some(entity);
    }

    public async Task<Option<Role>> GetById(RoleId id, CancellationToken cancellationToken)
    {
        var entity = await context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Role>() : Option.Some(entity);
    }

    public async Task<Role> Add(Role role, CancellationToken cancellationToken)
    {
        await context.Roles.AddAsync(role, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return role;
    }

    public async Task<Role> Update(Role role, CancellationToken cancellationToken)
    {
        context.Roles.Update(role);

        await context.SaveChangesAsync(cancellationToken);

        return role;
    }

    public async Task<Role> Delete(Role role, CancellationToken cancellationToken)
    {
        context.Roles.Remove(role);

        await context.SaveChangesAsync(cancellationToken);

        return role;
    }
}