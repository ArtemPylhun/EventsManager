using FluentValidation;

namespace Application.Events.Commands;

public class DeleteEventCommandValidator : AbstractValidator<DeleteEventCommand>
{
    public DeleteEventCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
    }
}