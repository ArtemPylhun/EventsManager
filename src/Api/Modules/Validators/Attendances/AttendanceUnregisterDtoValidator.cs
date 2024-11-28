using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Attendances;

public class AttendanceUnregisterDtoValidator : AbstractValidator<AttendanceUnregisterDto>
{
    public AttendanceUnregisterDtoValidator()
    {
        RuleFor(a => a.EventId).NotEmpty();
        RuleFor(a => a.UserId).NotEmpty();
    }
}