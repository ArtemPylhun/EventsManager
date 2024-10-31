using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Profiles;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record UpdateUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public string? FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Address { get; init; }
    public DateTime? BirthDate { get; init; }
}

public class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IUserQueries userQueries,
    IProfileQueries profileQueries) : IRequestHandler<UpdateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var existingUser = await userQueries.GetById(userId, cancellationToken);

        return await existingUser.Match(
            async u =>
            {
                var existingProfile = await profileQueries.GetById(u.ProfileId, cancellationToken);
                return await existingProfile.Match(
                    async p =>
                    {
                        var existingUserWithSameName =
                            await userQueries.SearchByUserName(request.UserName, cancellationToken);
                        return await existingUserWithSameName.Match(
                            un => Task.FromResult<Result<User, UserException>>(
                                new UserWithNameAlreadyExistsException(userId)),
                            async () => await UpdateEntity(u, request.UserName, request.Password, request.FullName,
                                request.PhoneNumber, request.Address, request.BirthDate, p, cancellationToken));
                    },
                    () => Task.FromResult<Result<User, UserException>>(
                        new UserProfileNotFoundException(userId, u.ProfileId)));
            },
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<User, UserException>> UpdateEntity(
        User entity,
        string? userName,
        string? password,
        string? fullName,
        string? phoneNumber,
        string? address,
        DateTime? birthDate,
        Profile profile,
        CancellationToken cancellationToken)
    {
        try
        {
            profile.UpdateDetails(fullName, birthDate, phoneNumber, address);
            entity.UpdateDetails(userName, entity.Email, password);
            return await userRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(entity.Id, exception);
        }
    }
}