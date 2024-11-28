using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Users;

public class UpdateUserRoleDtoValidator : AbstractValidator<UpdateUserRoleDto>
{
    public UpdateUserRoleDtoValidator()
    {
        RuleFor(u => u.UserId).NotEmpty();
        RuleFor(u => u.RoleId).NotEmpty();
    }
}