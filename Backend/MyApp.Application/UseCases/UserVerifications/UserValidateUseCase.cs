using Microsoft.Extensions.Logging;
using MyApp.Application.DTOs.Users;
using MyApp.Application.Interfaces.UseCases.UserVerifications;
using MyApp.Application.Validators.Users;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using MyApp.Shared.Services;

namespace MyApp.Application.UseCases.UserVerifications
{
    public class UserValidateUseCase : IUserValidateUseCase
    {
        private readonly IGenericRepository<UserVerificationsEntity> _userVerificationRepository;
        private readonly IGenericRepository<UsersEntity> _userRepository;
        private readonly ILogger<UserValidateUseCase> _logger;

        public UserValidateUseCase(
            IGenericRepository<UserVerificationsEntity> userVerificationRepository,
            IGenericRepository<UsersEntity> userRepository,
            ILogger<UserValidateUseCase> logger)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userVerificationRepository = userVerificationRepository;
        }

        public async Task<bool> Execute(UserCodeValidationRequest request)
        {
            _logger.LogInformation("Iniciando validación del usuario por código de verificación.");

            var validator = new UserCodeValidationValidator();
            ValidatorHelper.ValidateAndThrow(request, validator);

            UserVerificationsEntity? searchUserVerifications = await _userVerificationRepository.GetByCondition(
                x => x.CodeValidation == request.CodeValidation && x.IsUsed == false
            );

            if (searchUserVerifications is null)
            {
                _logger.LogWarning("Intento fallido de validación con código {CodeValidation}.", request.CodeValidation);
                throw new NotFoundException("El código ingresado es incorrecto.");
            }

            if (searchUserVerifications.CodeValidationExpiration < DateTime.UtcNow)
            {
                _logger.LogWarning("El código de verificación ha expirado para el usuario con codigo {Codigo}", searchUserVerifications.CodeValidationExpiration);
                throw new ConflictException("El código ha expirado. Solicita uno nuevo.");
            }

            var searchUser = await _userRepository.GetByCondition(x => x.UserId == searchUserVerifications.UserId);

            if (searchUser is null)
            {
                _logger.LogWarning("No se encontró ningún usuario asociado al código de verificación {CodeValidation}", request.CodeValidation);
                throw new NotFoundException("El usuario asociado a este código no existe o ha sido eliminado.");
            }

            searchUser.IsVerified = true;
            searchUser.UpdatedAt = DateTime.UtcNow;
            searchUserVerifications.IsUsed = true;
            searchUserVerifications.UpdatedAt = DateTime.UtcNow;

            UserVerificationsEntity? updateEntity = await _userVerificationRepository.Update(searchUserVerifications);
            UsersEntity? updateUser = await _userRepository.Update(searchUser);

            _logger.LogInformation("Usuario con código de verificación {CodeValidation} validado exitosamente.", request.CodeValidation);

            return true;
        }
    }
}
