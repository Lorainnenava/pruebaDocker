using FluentValidation;
using FluentValidation.Results;

namespace MyApp.Shared.Services
{
    public class ValidatorHelper
    {
        public static void ValidateAndThrow<T>(T instance, IValidator<T> validator)
        {
            ValidationResult result = validator.Validate(instance);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}
