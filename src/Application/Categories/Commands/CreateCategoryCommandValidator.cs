using FluentValidation;

namespace Application.Categories.Commands;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.Description).MinimumLength(10).MaximumLength(1000);
    }
}