using FluentValidation;
using MyApp.Application.DTOs.UserPasswordResets;

namespace MyApp.Application.Validators.UserPasswordResets
{
    public class UserPasswordResetValidateResetCodeValidator : AbstractValidator<UserPasswordResetValidateResetCodeRequest>
    {
        public UserPasswordResetValidateResetCodeValidator()
        {
            RuleFor(x => x.ResetPasswordCode)
                .NotEmpty().WithMessage("El campo ResetPasswordCode es obligatorio.")
                .Length(5).WithMessage("El campo ResetPasswordCode debe tener exactamente 5 dígitos.")
                .Matches(@"^\d{5}$").WithMessage("El campo ResetPasswordCode debe contener solo números.");
        }
    }
}
