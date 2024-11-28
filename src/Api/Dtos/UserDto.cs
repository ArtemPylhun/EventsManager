using Domain.Roles;
using Domain.Users;

namespace Api.Dtos;

public record UserCreateDto(
    string Email,
    string UserName,
    string Password)
{
    public static UserCreateDto FromUserCreateDomainModel(User user)
        => new(
            Email: user.Email,
            UserName: user.UserName,
            Password: user.PasswordHash
        );
}

public record UserDto(
    Guid? Id,
    string? Email,
    string? UserName,
    string? Password,
    ProfileDto? Profile,
    Guid? RoleId,
    RoleDto? Role)
{
    public static UserDto FromDomainModel(User user)
        => new(
            Id: user.Id.Value,
            Email: user.Email,
            UserName: user.UserName,
            Password: user.PasswordHash,
            Profile: user.Profile == null ? null : ProfileDto.FromDomainModel(user.Profile),
            RoleId: user.RoleId.Value,
            Role: user.Role == null ? null : RoleDto.FromDomainModel(user.Role));
}

public record UserUpdateDto(
    Guid Id,
    string? UserName,
    string? Password,
    string? FullName,
    string? PhoneNumber,
    string? Address,
    DateTime? BirthDate)
{
    public static UserUpdateDto FromUserUpdateDomainModel(User user)
        => new(
            Id: user.Id.Value,
            UserName: user.UserName,
            Password: user.PasswordHash,
            FullName: user.Profile!.FullName,
            PhoneNumber: user.Profile.PhoneNumber,
            Address: user.Profile.Address,
            BirthDate: user.Profile.BirthDate
        );
}

public record UpdateUserRoleDto(
    Guid UserId,
    Guid RoleId)
{
    public static UpdateUserRoleDto FromUserUpdateDomainModel(User user)
        => new(
            UserId: user.Id.Value,
            RoleId: user.RoleId.Value
        );
}

public record UserLoginDto(
    string Email,
    string Password)
{
}