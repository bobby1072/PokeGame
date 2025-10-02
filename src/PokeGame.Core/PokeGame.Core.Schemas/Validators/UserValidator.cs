using System.Net.Mail;
using FluentValidation;
using PokeGame.Core.Schemas;

namespace PokeGame.Core.Schemas.Validators;

internal sealed class UserValidator: BaseValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(user => user.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(user => user.Email).Must(IsValidEmail).WithMessage("Invalid email address");
    }
}