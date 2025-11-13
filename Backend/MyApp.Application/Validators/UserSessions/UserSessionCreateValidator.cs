using FluentValidation;
using MyApp.Application.DTOs.UserSessions;

namespace MyApp.Application.Validators.UserSessions
{
    public class UserSessionCreateValidator : AbstractValidator<UserSessionRequest>
    {
        public UserSessionCreateValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El campo Email es requerido.")
                .EmailAddress().WithMessage("El campo Email no es un correo electrónico válido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("El campo Password es requerido.")
                .Length(4, 10).WithMessage("Password debe tener entre 4 y 10 caracteres.");

            RuleFor(x => x.IpAddress)
                            .NotEmpty().WithMessage("El campo IpAddress es requerido.")
                            .Matches(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$")
                            .WithMessage("El campo IpAddress debe ser una dirección IPv4 válida.");
        }
    }
}
