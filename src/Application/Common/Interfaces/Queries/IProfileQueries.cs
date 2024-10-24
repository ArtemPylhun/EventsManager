using Domain.Profiles;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IProfileQueries
{
    Task<IReadOnlyList<Profile>> GetAll(CancellationToken cancellationToken);
    //Task<Option<Profile>> SearchByName(string name, CancellationToken cancellationToken);
    Task<Option<Profile>> GetById(ProfileId id, CancellationToken cancellationToken);
}