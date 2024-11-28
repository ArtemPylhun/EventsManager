using FluentValidation;

namespace Application.Users.Commands;

public class UnregisterUserFromEventCommandValidator : AbstractValidator<UnregisterUserFromEventCommand>
{
    public UnregisterUserFromEventCommandValidator()
    {
        RuleFor(a => a.UserId).NotEmpty();
        RuleFor(a => a.EventId).NotEmpty();
    }
}