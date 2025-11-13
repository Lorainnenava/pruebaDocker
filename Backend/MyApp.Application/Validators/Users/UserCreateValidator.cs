using FluentValidation;
using MyApp.Application.DTOs.Users;

namespace MyApp.Application.Validators.Users
{
    public class UserCreateValidator : AbstractValidator<UserCreateRequest>
    {
        public UserCreateValidator()
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

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("El campo Password es requerido.")
                .Length(4, 10).WithMessage("Password debe tener entre 4 y 10 caracteres.");

            RuleFor(x => x.IdentificationNumber)
                .NotEmpty().WithMessage("El campo IdentificatiónNumber es requerido.")
                .MaximumLength(20).WithMessage("IdentificatiónNumber debe tener máximo 20 caracteres.");

            RuleFor(x => x.IdentificationTypeId)
                .NotEmpty().WithMessage("El campo IdentificationTypeId es requerido.");

            RuleFor(x => x.GenderId)
                .NotEmpty().WithMessage("El campo GenderId es requerido.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("El campo DateOfBirth es requerido.")
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("La fecha de nacimiento debe ser menor o igual a la fecha actual.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("El campo Phone es requerido.")
                .Length(10).WithMessage("Phone debe tener exactamente 10 caracteres.");
        }
    }
}
