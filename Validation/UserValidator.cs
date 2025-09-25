using ApiLabo.Data.Models;
using ApiLabo.Dto;
using FluentValidation;

namespace ApiLabo.Validation;

public class UserValidator: AbstractValidator<UserInputModel>
{
    public UserValidator()
    {
        RuleFor(user => user.Pseudo).NotEmpty().WithMessage("Pseudo required :(");
        RuleFor(user => user.Password).NotEmpty().WithMessage("Password required :(");
        RuleFor(user => user.Birthday).LessThanOrEqualTo(DateTime.Now);
    }
}
