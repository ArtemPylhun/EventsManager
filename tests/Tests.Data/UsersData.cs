using Domain.Profiles;
using Domain.Roles;
using Domain.Users;

namespace Tests.Data;

public static class UsersData
{
    public static User MainUser(RoleId roleId, ProfileId profileId)
        => User.New(UserId.New(), "UserName", "userName@gmail.com", "la1sklj5jsa3g2lksg", DateTime.UtcNow, roleId,
            profileId);
    
    public static User NewUser(RoleId roleId)
        => User.New(UserId.New(), "UserName", "userName@gmail.com", "la1sklj5jsa3g2lksg", DateTime.UtcNow, roleId, ProfileId.Empty());
}