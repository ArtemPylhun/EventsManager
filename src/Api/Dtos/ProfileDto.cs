using Domain.Profiles;

namespace Api.Dtos;

public record ProfileDto(
    Guid? Id,
    string? FullName,
    DateTime? BirthDate,
    string? PhoneNumber,
    string? Address
)
{
    public static ProfileDto FromDomainModel(Profile profile)
        => new(
            Id: profile.Id.Value,
            FullName: profile.FullName,
            BirthDate: profile.BirthDate,
            PhoneNumber: profile.PhoneNumber,
            Address: profile.Address
        );
}