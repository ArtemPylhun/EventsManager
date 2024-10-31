using Domain.Roles;

namespace Tests.Data;

public class RolesData
{
    public static Role UserRole => Role.New(RoleId.New(), "User");
    public static Role AdminRole => Role.New(RoleId.New(), "Admin");
    public static Role NoUsersRole => Role.New(RoleId.New(), "No users");
}