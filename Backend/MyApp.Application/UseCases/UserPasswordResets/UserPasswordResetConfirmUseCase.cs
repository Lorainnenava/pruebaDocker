using Microsoft.Extensions.Logging;
using MyApp.Application.DTOs.UserPasswordResets;
using MyApp.Application.Interfaces.Infrastructure;
using MyApp.Application.Interfaces.UseCases.UserPasswordResets;
using MyApp.Application.Validators.UserPasswordResets;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using MyApp.Shared.Services;

namespace MyApp.Application.UseCases.UserPasswordResets
{
    public class UserPasswordResetConfirmUseCase : IUserPasswordResetConfirmUseCase
    {
        private readonly ILogger<UserPasswordResetConfirmUseCase> _logger;
        private readonly IGenericRepository<UsersEntity> _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IGenericRepository<UserPasswordResetsEntity> _userPasswordResetRepository;

        public UserPasswordResetConfirmUseCase(
            IPasswordHasherService passwordHasherService,
            IGenericRepository<UsersEntity> userRepository,
            ILogger<UserPasswordResetConfirmUseCase> logger,
            IGenericRepository<UserPasswordResetsEntity> userPasswordResetRepository
            )
        {
            _logger = logger;
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _userPasswordResetRepository = userPasswordResetRepository;
        }

        public async Task<bool> Execute(UserPasswordResetConfirmRequest request)
        {
            _logger.LogInformation("Iniciando el cambio de contraseña para el usuario con Email: {Email}", request.Email);

            var validator = new UserPasswordResetConfirmValidator();
            ValidatorHelper.ValidateAndThrow(request, validator);

            var dbContext = _userRepository.GetDbContext();

            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            var searchUser = await _userRepository.GetByCondition(x => x.Email == request.Email);

            if (searchUser is null)
            {
                _logger.LogWarning("No se encontró ningún usuario con Email: {Email}", request.Email);
                throw new NotFoundException("No existe un usuario asociado a este correo.");
            }

            if (!searchUser.IsActive || !searchUser.IsVerified)
            {
                _logger.LogWarning("El usuario con el email {Email} está inactivo o no ha validado su cuenta", request.Email);
                throw new NotFoundException("Cuenta inactiva o no verificada.");
            }

            var resetConfirmation = await _userPasswordResetRepository.GetByCondition(x =>
                x.UserId == searchUser.UserId &&
                x.ResetPasswordCode == request.ResetPasswordCode &&
                x.IsUsed == false);

            if (resetConfirmation is null)
            {
                _logger.LogWarning("Código de restablecimiento inválido o ya utilizado para el usuario con Email: {Email}", request.Email);
                throw new InvalidOperationException("El código de restablecimiento es inválido o ya ha sido utilizado.");
            }

            if (resetConfirmation.ResetPasswordCodeExpiration < DateTime.UtcNow)
            {
                _logger.LogWarning("El código de resetear contraseña ha expirado para el usuario con codigo {Codigo}", resetConfirmation.ResetPasswordCode);
                throw new ConflictException("El código ha expirado. Solicita uno nuevo.");
            }

            bool isSamePassword = _passwordHasherService.VerifyPassword(request.NewPassword, searchUser.PasswordHash);

            if (isSamePassword)
            {
                _logger.LogWarning("El usuario con ID: {UserId} intentó cambiar la contraseña por la misma actual", searchUser.UserId);
                throw new InvalidOperationException("La nueva contraseña no puede ser igual a la anterior.");
            }

            try
            {
                searchUser.PasswordHash = _passwordHasherService.HashPassword(request.NewPassword);
                searchUser.UpdatedAt = DateTime.UtcNow;
                await _userRepository.Update(searchUser);


                resetConfirmation.IsUsed = true;
                resetConfirmation.UpdatedAt = DateTime.UtcNow;
                await _userPasswordResetRepository.Update(resetConfirmation);

                await transaction.CommitAsync();
                _logger.LogInformation("Contraseña cambiada exitosamente para el usuario con Email: {Email}", request.Email);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al cambiar la contraseña para el usuario con Email: {Email}", request.Email);
                throw new Exception("Ocurrió un error al cambiar la contraseña. Por favor, inténtelo de nuevo más tarde.");
            }
        }
    }
}
