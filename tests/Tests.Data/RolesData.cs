using Domain.Roles;

namespace Tests.Data;

public class RolesData
{
    public static Role UserRole => Role.New(RoleId.New(), "User");
    public static Role NoUsersRole => Role.New(RoleId.New(), "No users");
}