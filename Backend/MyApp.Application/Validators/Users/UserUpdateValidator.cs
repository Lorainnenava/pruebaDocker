using FluentValidation;
using MyApp.Application.DTOs.Users;

namespace MyApp.Application.Validators.Users
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateRequest>
    {
        public UserUpdateValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El campo FirstName es requerido.")
                .Length(4, 20).WithMessage("FirstName debe tener entre 4 y 20 caracteres.");

            RuleFor(x => x.MiddleName)
                .MaximumLength(20).WithMessage("MiddleName debe tener máximo 20 caracteres.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El campo LastName es requerido.")
                .Length(4, 20).WithMessage("LastName debe tener entre 4 y 20 caracteres.");

            RuleFor(x => x.SecondName)
                .MaximumLength(20).WithMessage("SecondName debe tener máximo 20 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El campo Email es requerido.")
                .EmailAddress().WithMessage("El campo Email no es un correo electrónico válido.");

            RuleFor(x => x.IdentificationTypeId)
                .NotEmpty().WithMessage("El campo IdentificationTypeId es requerido.");

            RuleFor(x => x.GenderId)
                .NotEmpty().WithMessage("El campo GenderId es requerido.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("El campo Phone es requerido.")
                .Length(10).WithMessage("Phone debe tener exactamente 10 caracteres.");
        }
    }
}
