﻿using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Events;

public class EventDtoValidator : AbstractValidator<EventDto>
{
    public EventDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(5).MaximumLength(255);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(5).MaximumLength(1000);
        RuleFor(x => x.StartDate).NotEmpty().GreaterThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.EndDate).NotEmpty().GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.OrganizerId).NotEmpty();
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}