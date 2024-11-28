using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Locations;

public class LocationDtoValidator : AbstractValidator<LocationDto>
{
    public LocationDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.Address).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.City).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.Country).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.Capacity).NotEmpty().GreaterThanOrEqualTo(1);
    }
}