using Microsoft.Extensions.Logging;
using MyApp.Application.DTOs.UserVerifications;
using MyApp.Application.Interfaces.Infrastructure;
using MyApp.Application.Interfaces.Services;
using MyApp.Application.Interfaces.UseCases.UserVerifications;
using MyApp.Application.Validators.UserVerifications;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using MyApp.Shared.Services;

namespace MyApp.Application.UseCases.UserVerifications
{
    public class ResendVerificationCodeUseCase : IResendVerificationCodeUseCase
    {
        private readonly IEmailService _emailService;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly ILogger<ResendVerificationCodeUseCase> _logger;
        private readonly IGenericRepository<UsersEntity> _userRepository;
        private readonly IGenericRepository<UserVerificationsEntity> _userVerificationRepository;

        public ResendVerificationCodeUseCase(
            IEmailService emailService,
            ICodeGeneratorService codeGeneratorService,
            ILogger<ResendVerificationCodeUseCase> logger,
            IGenericRepository<UsersEntity> userRepository,
            IGenericRepository<UserVerificationsEntity> userVerificationRepository)
        {
            _logger = logger;
            _emailService = emailService;
            _userRepository = userRepository;
            _codeGeneratorService = codeGeneratorService;
            _userVerificationRepository = userVerificationRepository;
        }

        public async Task<bool> Execute(ResendCodeRequest request)
        {
            _logger.LogInformation("Solicitando reenvío del código de verificación para el usuario con correo: {Email}.", request.Email);

            var dbContext = _userRepository.GetDbContext();

            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            var validator = new UserVerificationResendCodeValidator();
            ValidatorHelper.ValidateAndThrow(request, validator);

            var searchUser = await _userRepository.GetByCondition(x => x.Email == request.Email);

            if (searchUser is null)
            {
                _logger.LogWarning("Intento de reenvío de código fallido. No se encontró ningún usuario con el correo: {Email}.", request.Email);
                throw new NotFoundException("No se encontró un usuario asociado a este correo.");
            }

            if (searchUser.IsVerified)
            {
                _logger.LogWarning("Intento de reenviar código fallido: el usuario con el correo {Email} ya ha verificado su cuenta.", request.Email);
                throw new ConflictException("Este usuario ya ha verificado su cuenta.");
            }

            var searchUserVerifications = await _userVerificationRepository.GetAll(x => x.UserId == searchUser.UserId && x.IsUsed == false);

            if (searchUserVerifications.Any())
            {
                foreach (var verification in searchUserVerifications)
                {
                    verification.IsUsed = true;
                    verification.UpdatedAt = DateTime.UtcNow;
                    await _userVerificationRepository.Update(verification);
                }
            }

            var codeValidation = await _codeGeneratorService.GenerateUniqueCode();

            var newEntity = new UserVerificationsEntity
            {
                UserId = searchUser.UserId,
                CodeValidation = codeValidation,
                CodeValidationExpiration = DateTime.UtcNow.AddMinutes(15),
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var updateUser = await _userVerificationRepository.Create(newEntity);

                await _emailService.SendEmail(
                    request.Email,
                    "Verificación de cuenta",
                    "..\\MyApp.Shared\\TemplateEmails\\VerificationTemplate.cshtml",
                    new Dictionary<string, string>
                    {
                        { "FirstName", searchUser.FirstName },
                        { "CodeValidation", codeValidation },
                });

                await transaction.CommitAsync();
                _logger.LogInformation("Se ha reenviado el codigo de verificacion al usuario con el correo {Email}", request.Email);

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Ocurrió un error durante el reenvio del codigo de verificación del usuario. Se realizó rollback.");
                throw;
            }
        }
    }
}
