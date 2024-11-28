using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators.Users;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(255);
        RuleFor(x => x.Password).NotEmpty()
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$").MinimumLength(8);
    }
}