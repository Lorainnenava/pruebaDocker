using FluentValidation;
using MyApp.Application.DTOs.Users;

namespace MyApp.Application.Validators.Users
{
    public class UserChangePasswordValidator : AbstractValidator<UserChangePasswordRequest>
    {
        public UserChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("El campo CurrentPassword es requerido.")
                .Length(4, 10).WithMessage("CurrentPassword debe tener entre 4 y 10 caracteres.");
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("El campo NewPassword es requerido.")
                .Length(4, 10).WithMessage("NewPassword debe tener entre 4 y 10 caracteres.");
        }
    }
}
