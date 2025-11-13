using Microsoft.Extensions.Logging;
using MyApp.Application.DTOs.UserPasswordResets;
using MyApp.Application.Interfaces.Infrastructure;
using MyApp.Application.Interfaces.Services;
using MyApp.Application.Interfaces.UseCases.UserPasswordResets;
using MyApp.Application.Validators.UserPasswordResets;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using MyApp.Shared.Services;

namespace MyApp.Application.UseCases.UserPasswordResets
{
    public class UserPasswordResetRequestUseCase : IUserPasswordResetRequestUseCase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<UserPasswordResetRequestUseCase> _logger;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IGenericRepository<UsersEntity> _userRepository;
        private readonly IGenericRepository<UserPasswordResetsEntity> _userPasswordResetRepository;

        public UserPasswordResetRequestUseCase(
            IEmailService emailService,
            ICodeGeneratorService codeGeneratorService,
            IGenericRepository<UsersEntity> userRepository,
            IGenericRepository<UserPasswordResetsEntity> userPasswordResetRepository,
            ILogger<UserPasswordResetRequestUseCase> logger)
        {
            _logger = logger;
            _emailService = emailService;
            _userRepository = userRepository;
            _codeGeneratorService = codeGeneratorService;
            _userPasswordResetRepository = userPasswordResetRepository;
        }

        public async Task<bool> Execute(UserPasswordResetRequest request)
        {
            _logger.LogInformation("Iniciando el cambio de contraseña para el usuario con Email: {Email}", request.Email);

            var validator = new UserPasswordResetRequestValidator();
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

            var pendingResetRequests = await _userPasswordResetRepository
                .GetAll(x => x.UserId == searchUser.UserId && x.IsUsed == false);

            if (pendingResetRequests.Any())
            {
                foreach (var reset in pendingResetRequests)
                {
                    reset.IsUsed = true;
                    reset.UpdatedAt = DateTime.UtcNow;
                    await _userPasswordResetRepository.Update(reset);
                }
            }

            var codeValidation = await _codeGeneratorService.GenerateCodeByResetPassword();

            try
            {
                await _emailService.SendEmail(
                request.Email,
                "Restablecimiento de Contraseña",
                "..\\MyApp.Shared\\TemplateEmails\\ForgotPasswordTemplate.cshtml",
                new Dictionary<string, string>
                {
                { "CodeValidation", codeValidation },
                });

                var newEntity = new UserPasswordResetsEntity
                {
                    UserId = searchUser.UserId,
                    ResetPasswordCode = codeValidation,
                    CreatedAt = DateTime.UtcNow,
                    ResetPasswordCodeExpiration = DateTime.UtcNow.AddMinutes(15)
                };

                var resetPassword = await _userPasswordResetRepository.Create(newEntity);

                await _userRepository.Update(searchUser);

                await transaction.CommitAsync();
                _logger.LogInformation("Correo enviado exitosamente al usuario con el Email {Email}", request.Email);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al enviar el correo de restablecimiento de contraseña a {Email}", request.Email);
                throw new Exception("Ocurrió un error al procesar la solicitud. Por favor, inténtelo de nuevo más tarde.");
            }
        }
    }
}
