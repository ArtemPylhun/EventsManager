using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Categories;

public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(x => x.Name).MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.Description).MinimumLength(5).MaximumLength(1000);
    }
}