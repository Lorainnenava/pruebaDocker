using FluentValidation;
using MyApp.Application.DTOs.UserPasswordResets;

namespace MyApp.Application.Validators.UserPasswordResets
{
    public class UserPasswordResetRequestValidator : AbstractValidator<UserPasswordResetRequest>
    {
        public UserPasswordResetRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El campo Email es requerido.")
                .EmailAddress().WithMessage("El campo Email no es un correo electrónico válido.");
        }
    }
}
