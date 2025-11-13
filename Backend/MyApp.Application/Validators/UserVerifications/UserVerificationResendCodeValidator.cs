using FluentValidation;
using MyApp.Application.DTOs.UserVerifications;

namespace MyApp.Application.Validators.UserVerifications
{
    public class UserVerificationResendCodeValidator : AbstractValidator<ResendCodeRequest>
    {
        public UserVerificationResendCodeValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El campo Email es requerido.")
                .EmailAddress().WithMessage("El campo Email no es un correo electrónico válido.");
        }
    }
}
