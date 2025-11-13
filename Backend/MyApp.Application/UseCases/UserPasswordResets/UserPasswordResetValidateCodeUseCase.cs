using Microsoft.Extensions.Logging;
using MyApp.Application.DTOs.UserPasswordResets;
using MyApp.Application.Interfaces.UseCases.UserPasswordResets;
using MyApp.Application.Validators.UserPasswordResets;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using MyApp.Shared.Services;

namespace MyApp.Application.UseCases.UserPasswordResets
{
    public class UserPasswordResetValidateCodeUseCase : IUserPasswordResetValidateCodeUseCase
    {
        private readonly IGenericRepository<UserPasswordResetsEntity> _userPasswordResetRepository;
        private readonly ILogger<UserPasswordResetValidateCodeUseCase> _logger;

        public UserPasswordResetValidateCodeUseCase(
            IGenericRepository<UserPasswordResetsEntity> userPasswordResetRepository,
            ILogger<UserPasswordResetValidateCodeUseCase> logger)
        {
            _logger = logger;
            _userPasswordResetRepository = userPasswordResetRepository;
        }

        public async Task<bool> Execute(UserPasswordResetValidateResetCodeRequest request)
        {
            _logger.LogInformation("Iniciando validación del código de restablecimiento de contraseña.");

            var validator = new UserPasswordResetValidateResetCodeValidator();
            ValidatorHelper.ValidateAndThrow(request, validator);

            UserPasswordResetsEntity? isExists = await _userPasswordResetRepository.GetByCondition(
                x => x.ResetPasswordCode == request.ResetPasswordCode && x.IsUsed == false, x => x.User
            );

            if (isExists is null)
            {
                _logger.LogWarning("Intento fallido de validación con código {CodeValidation}.", request.ResetPasswordCode);
                throw new NotFoundException("El código ingresado es incorrecto.");
            }

            if (isExists.ResetPasswordCodeExpiration < DateTime.UtcNow)
            {
                _logger.LogWarning("El código de resetear contraseña ha expirado para el usuario con codigo {Codigo}", isExists.ResetPasswordCode);
                throw new ConflictException("El código ha expirado. Solicita uno nuevo.");
            }

            _logger.LogInformation("El código {CodeValidation} fue validado correctamente para el usuario con el ID {UserId}.", request.ResetPasswordCode, isExists.UserId);

            return true;
        }
    }
}
