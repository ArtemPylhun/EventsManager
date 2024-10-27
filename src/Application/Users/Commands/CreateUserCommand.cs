using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Profiles;
using Domain.Roles;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record CreateUserCommand : IRequest<Result<User, UserException>>
{
    public required string Email { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
    public required Guid RoleId { get; init; }
}

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IUserQueries userQueries,
    IRoleQueries roleQueries,
    IProfileRepository profileRepository)
    : IRequestHandler<CreateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var roleId = new RoleId(request.RoleId);

        var role = await roleQueries.GetById(roleId, cancellationToken);

        return await role.Match<Task<Result<User, UserException>>>(
            async r =>
            {
                var existingUserWithUserName = await userQueries.SearchByUserName(
                    request.UserName,
                    cancellationToken);


                return await existingUserWithUserName.Match(
                    un => Task.FromResult<Result<User, UserException>>(new UserWithNameAlreadyExistsException(un.Id)),
                    async () =>
                    {
                        var existingUserWithEmail = await userQueries.SearchByEmail(
                            request.Email,
                            cancellationToken);
                        return await existingUserWithEmail.Match<Task<Result<User, UserException>>>(
                            ue => Task.FromResult<Result<User, UserException>>(
                                new UserWithEmailAlreadyExistsException(ue.Id)),
                            async () => await CreateEntity(request.UserName, request.Email, request.Password,
                                request.RoleId, cancellationToken));
                    });
            },
            () => Task.FromResult<Result<User, UserException>>(new UserRoleNotFoundException(UserId.Empty(), roleId)));
    }

    private async Task<Result<User, UserException>> CreateEntity(
        string userName,
        string email,
        string password,
        Guid roleId,
        CancellationToken cancellationToken)
    {
        try
        {
            var usersProfile = Profile.New(ProfileId.New(), null, DateTime.UtcNow, string.Empty, String.Empty);
            await profileRepository.Add(usersProfile, cancellationToken);
            var passwordHash = password; //TODO: Password hashing 
            var entity = User.New(UserId.New(), userName, email, passwordHash, DateTime.UtcNow, new RoleId(roleId),
                usersProfile.Id);

            return await userRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(UserId.Empty(), exception);
        }
    }
}