using Domain.Profiles;

namespace Tests.Data;

public class ProfilesData
{
    public static Profile MainProfile => Profile.New(ProfileId.New(), "Full User Name", DateTime.UtcNow.AddYears(-18),
        "+38987654321", "city New York, Boulevard street");
}