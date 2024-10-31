using Domain.Profiles;
using Domain.Roles;
using Domain.Users;

namespace Tests.Data;

public static class UsersData
{
    public static string passwordHash = BCrypt.Net.BCrypt.HashPassword("password", BCrypt.Net.BCrypt.GenerateSalt());

    public static User MainUser(RoleId roleId, ProfileId profileId)
        => User.New(UserId.New(), "UserName", "userName@gmail.com", passwordHash, DateTime.UtcNow, roleId,
            profileId);

    public static User NewUser(RoleId roleId)
        => User.New(UserId.New(), "UserName", "userName@gmail.com", passwordHash, DateTime.UtcNow, roleId,
            ProfileId.Empty());
}