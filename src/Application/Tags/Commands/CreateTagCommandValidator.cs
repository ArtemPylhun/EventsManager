using FluentValidation;

namespace Application.Tags.Commands;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255).MinimumLength(3);
    }
}