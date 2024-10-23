using Domain.Attendances;
using Domain.Profiles;
using Domain.Roles;

namespace Domain.Users;

public class User
{
    public UserId Id { get; }
    public string UserName { get; private set; }
    public string Email { get; private set;}
    public string PasswordHash {get; private set;}
    public DateTime RegisteredOn { get; }
    public RoleId RoleId { get; }
    public Role? Role { get; }
    public ProfileId ProfileId { get; }
    public Profile? Profile { get; }

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    private User(UserId userId, string userName, string email, string passwordHash, DateTime registeredOn, RoleId roleId, ProfileId profileId)
    {
        Id = userId;
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
        RegisteredOn = registeredOn;
        RoleId = roleId;
        ProfileId = profileId;
    }
    
    public static User New(UserId userId, string userName, string email, string passwordHash, DateTime registeredOn, RoleId roleId, ProfileId profileId)
        => new(userId, userName, email, passwordHash, registeredOn, roleId, profileId);

    public void UpdateDetails(string userName, string email, string passwordHash)
    {
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
    }
}