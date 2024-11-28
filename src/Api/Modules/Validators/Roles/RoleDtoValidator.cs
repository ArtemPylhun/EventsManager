using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Roles;

public class RoleDtoValidator : AbstractValidator<RoleDto>
{
    public RoleDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(3).MaximumLength(255);
    }
}