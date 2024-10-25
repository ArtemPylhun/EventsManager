using Domain.Users;

namespace Api.Dtos;

public record UserCreateDto(
    string Email,
    string UserName,
    string Password,
    Guid RoleId)
{
    public static UserCreateDto FromUserCreateDomainModel(User user)
        => new(
            Email: user.Email,
            UserName: user.UserName,
            Password: user.PasswordHash,
            RoleId: user.RoleId.Value
        );
}