using FluentValidation;

namespace Application.Users.Commands;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.FullName).MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.BirthDate).LessThan(DateTime.UtcNow).WithMessage("Please enter a valid date");
        RuleFor(x => x.PhoneNumber); //TODO: Regex for phone number
        RuleFor(x => x.Address).MaximumLength(255).MinimumLength(3);
    }
}