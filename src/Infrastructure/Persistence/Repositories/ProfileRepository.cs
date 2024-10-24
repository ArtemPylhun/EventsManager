using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Profiles;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class ProfileRepository(ApplicationDbContext context): IProfileRepository, IProfileQueries
{
    public async Task<IReadOnlyList<Profile>> GetAll(CancellationToken cancellationToken)
    {
        return await context.Profiles
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /*public async Task<Option<Profile>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await context.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return entity == null ? Option.None<Profile>() : Option.Some(entity);
    }*/

    public async Task<Option<Profile>> GetById(ProfileId id, CancellationToken cancellationToken)
    {
        var entity = await context.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<Profile>() : Option.Some(entity);
    }

    public async Task<Profile> Add(Profile profile, CancellationToken cancellationToken)
    {
        await context.Profiles.AddAsync(profile, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return profile;
    }

    public async Task<Profile> Update(Profile profile, CancellationToken cancellationToken)
    {
        context.Profiles.Update(profile);

        await context.SaveChangesAsync(cancellationToken);

        return profile;
    }
    
    public async Task<Profile> Delete(Profile profile, CancellationToken cancellationToken)
    {
        context.Profiles.Remove(profile);

        await context.SaveChangesAsync(cancellationToken);

        return profile;
    }
}