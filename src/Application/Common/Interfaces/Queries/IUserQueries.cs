using Domain.Events;
using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IUserQueries
{
    Task<Option<User>> GetById(UserId id, CancellationToken cancellationToken);
    Task<Option<User>> SearchByEmailAndUsername(string email, string username, CancellationToken cancellationToken);
}