using FluentValidation;

namespace Application.Users.Commands;

public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
{
    public UpdateUserRoleCommandValidator()
    {
        RuleFor(u => u.UserId).NotEmpty();
        RuleFor(u => u.RoleId).NotEmpty();
    }
}