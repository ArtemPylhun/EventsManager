using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Tags;

public class TagDtoValidator : AbstractValidator<TagDto>
{
    public TagDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(3).MaximumLength(255);
    }
}