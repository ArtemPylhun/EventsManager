using Domain.Profiles;

namespace Tests.Data;

public class ProfilesData
{
    public static Profile MainProfile => Profile.New(ProfileId.New(), "Full User Name", DateTime.UtcNow.AddYears(-18),
        "+38987654321", "city New York, Boulevard street");

    public static Profile MainProfileTwo => Profile.New(ProfileId.New(), "Full User Two Name",
        DateTime.UtcNow.AddYears(-19),
        "+38987654321", "city New York, Boulevard street");

    public static Profile MainProfileThree => Profile.New(ProfileId.New(), "Full User Three Name",
        DateTime.UtcNow.AddYears(-20),
        "+38987654321", "city New York, Boulevard street");

    public static Profile NewProfile => Profile.New(ProfileId.New(), "Full New User Name",
        DateTime.UtcNow.AddYears(-22),
        "+38987656341", "Washington d. c., Washington street");
}