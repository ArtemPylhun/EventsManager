namespace Domain.Profiles;

public class Profile
{
    public ProfileId Id { get; }
    public string? FullName { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }

    private Profile(ProfileId id, string? fullName, DateTime? birthDate, string? phoneNumber, string? address)
    {
        Id = id;
        FullName = fullName;
        BirthDate = birthDate;
        PhoneNumber = phoneNumber;
        Address = address;
    }
    
    public static Profile New(ProfileId id, string? fullName, DateTime? birthDate, string? phoneNumber, string? address)
        => new(id, fullName, birthDate, phoneNumber, address);

    public void UpdateDetails(string fullName, DateTime birthDate, string? phoneNumber, string? address)
    {
        FullName = fullName;
        BirthDate = birthDate;
        PhoneNumber = phoneNumber;
        Address = address;
    }
}