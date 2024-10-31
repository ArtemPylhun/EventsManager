using Domain.Users;

namespace Domain.Roles;

public class Role
{
    public RoleId Id { get; }
    public string Title { get; private set; }

    private Role(RoleId id, string title)
    {
        Id = id;
        Title = title;
    }

    public static Role New(RoleId id, string title)
        => new(id, title);

    public void UpdateDetails(string title)
    {
        Title = title;
    }
}