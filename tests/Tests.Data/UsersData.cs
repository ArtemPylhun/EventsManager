using Domain.Profiles;
using Domain.Roles;
using Domain.Users;

namespace Tests.Data;

public static class UsersData
{
    public static string passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin!23", BCrypt.Net.BCrypt.GenerateSalt());

    public static User MainUser(RoleId roleId, ProfileId profileId)
        => User.New(UserId.New(), "UserName", "userName@gmail.com", passwordHash, DateTime.UtcNow, roleId,
            profileId);

    public static User MainUserTwo(RoleId roleId, ProfileId profileId)
        => User.New(UserId.New(), "UserName2", "userName@gmail.com", passwordHash, DateTime.UtcNow, roleId,
            profileId);

    public static User MainUserThree(RoleId roleId, ProfileId profileId)
        => User.New(UserId.New(), "UserName3", "userName@gmail.com", passwordHash, DateTime.UtcNow, roleId,
            profileId);

    public static User NewUser(RoleId roleId, ProfileId profileId)
        => User.New(UserId.New(), "UserName", "userName@gmail.com", passwordHash, DateTime.UtcNow, roleId,
            profileId);
}