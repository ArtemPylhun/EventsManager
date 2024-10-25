using Domain.Profiles;

namespace Application.Common.Interfaces.Repositories;

public interface IProfileRepository
{
    Task<Profile> Add(Profile profile, CancellationToken cancellationToken);
    Task<Profile> Update(Profile profile, CancellationToken cancellationToken);
    Task<Profile> Delete(Profile profile, CancellationToken cancellationToken);

}