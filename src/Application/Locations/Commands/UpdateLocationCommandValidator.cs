using FluentValidation;

namespace Application.Locations.Commands;

public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.Address).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.City).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.Country).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.Capacity).NotEmpty().GreaterThanOrEqualTo(1);
    }
}