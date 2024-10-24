using Domain.Profiles;

namespace Application.Common.Interfaces.Repositories;

public interface IProfileRepository
{
    Task<Profile> Add(Profile course, CancellationToken cancellationToken);
    Task<Profile> Update(Profile course, CancellationToken cancellationToken);
    Task<Profile> Delete(Profile course, CancellationToken cancellationToken);

}