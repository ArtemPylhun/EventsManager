using FluentValidation;

namespace Application.Events.Commands;

public class CancelEventCommandValidator : AbstractValidator<CancelEventCommand>
{
    public CancelEventCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
    }
}