using FluentValidation;

namespace Application.Users.Commands;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(u => u.UserName)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(u => u.Password)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .MinimumLength(8);

        RuleFor(u => u.FullName)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(u => u.PhoneNumber)
            .Matches(@"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$");

        RuleFor(u => u.Address)
            .MinimumLength(3)
            .MaximumLength(255);

        RuleFor(u => u.BirthDate)
            .LessThan(DateTime.UtcNow.AddYears(-16));
    }
}