using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Roles;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record LoginUserCommand : IRequest<Result<string, UserException>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginUserCommandHandler(
    IRoleQueries roleQueries,
    IUserQueries userQueries,
    IJwtProvider jwtProvider) : IRequestHandler<LoginUserCommand, Result<string, UserException>>
{
    public async Task<Result<string, UserException>> Handle(LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userQueries.SearchByEmail(request.Email, cancellationToken);
        return await user.Match(async u =>
            {
                if (!BCrypt.Net.BCrypt.Verify(request.Password, u.PasswordHash))
                {
                    return new InvalidCredentialsException();
                }

                var role = await roleQueries.GetById(u.RoleId, cancellationToken);
                return await role.Match<Task<Result<string, UserException>>>(
                    async r =>
                    {
                        string token = jwtProvider.Generate(u, r);
                        return token;
                    },
                    () => Task.FromResult<Result<string, UserException>>(
                        new UserRoleNotFoundException(UserId.Empty(), RoleId.Empty())));
            },
            () => Task.FromResult<Result<string, UserException>>(new InvalidCredentialsException()));
    }
}