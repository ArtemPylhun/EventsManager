using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Profiles;

public class ProfileDtoValidator : AbstractValidator<ProfileDto>
{
    public ProfileDtoValidator()
    {
        RuleFor(p => p.Address)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(p => p.PhoneNumber)
            .Matches(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$");

        RuleFor(p => p.BirthDate)
            .LessThan(DateTime.UtcNow.AddYears(-16));

        RuleFor(p => p.FullName)
            .MinimumLength(3)
            .MaximumLength(255);
    }
}