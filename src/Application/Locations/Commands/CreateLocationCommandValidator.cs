using FluentValidation;

namespace Application.Locations.Commands;

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.Address).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.City).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.Country).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.Capacity).NotEmpty().GreaterThanOrEqualTo(1);
    }
}