using Domain.Roles;

namespace Tests.Data;

public class RolesData
{
    public static Role MainRole => Role.New(RoleId.New(), "Admin");
}