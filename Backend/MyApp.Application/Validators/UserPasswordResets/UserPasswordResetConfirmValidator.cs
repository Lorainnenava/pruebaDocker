using FluentValidation;
using MyApp.Application.DTOs.UserPasswordResets;

namespace MyApp.Application.Validators.UserPasswordResets
{
    public class UserPasswordResetConfirmValidator : AbstractValidator<UserPasswordResetConfirmRequest>
    {
        public UserPasswordResetConfirmValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El campo Email es requerido.")
                .EmailAddress().WithMessage("El campo Email no es un correo electrónico válido.");
            RuleFor(x => x.ResetPasswordCode)
                .NotEmpty().WithMessage("El campo ResetPasswordCode es obligatorio.")
                .Length(5).WithMessage("El campo ResetPasswordCode debe tener exactamente 5 dígitos.")
                .Matches(@"^\d{5}$").WithMessage("El campo ResetPasswordCode debe contener solo números.");
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("El campo NewPassword es requerido.")
                .Length(4, 10).WithMessage("NewPassword debe tener entre 4 y 10 caracteres.");
        }
    }
}
