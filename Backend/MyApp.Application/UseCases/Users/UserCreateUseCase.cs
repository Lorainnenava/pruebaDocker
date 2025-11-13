using AutoMapper;
using Microsoft.Extensions.Logging;
using MyApp.Application.DTOs.Users;
using MyApp.Application.Interfaces.Infrastructure;
using MyApp.Application.Interfaces.Services;
using MyApp.Application.Interfaces.UseCases.Users;
using MyApp.Application.Validators.Users;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using MyApp.Shared.Services;

namespace MyApp.Application.UseCases.Users
{
    public class UserCreateUseCase : IUserCreateUseCase
    {
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserCreateUseCase> _logger;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IGenericRepository<UsersEntity> _userRepository;
        private readonly IGenericRepository<UserVerificationsEntity> _userVerificationRepository;

        public UserCreateUseCase(
            IMapper mapper,
            IEmailService emailService,
            ILogger<UserCreateUseCase> logger,
            ICodeGeneratorService codeGeneratorService,
            IPasswordHasherService passwordHasherService,
            IGenericRepository<UsersEntity> userRepository,
            IGenericRepository<UserVerificationsEntity> userVerification)
        {
            _logger = logger;
            _mapper = mapper;
            _emailService = emailService;
            _userRepository = userRepository;
            _codeGeneratorService = codeGeneratorService;
            _userVerificationRepository = userVerification;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<UserResponse> Execute(UserCreateRequest request)
        {
            _logger.LogInformation("Iniciando la creación de usuario con email: {Email}", request.Email);

            var validator = new UserCreateValidator();
            ValidatorHelper.ValidateAndThrow(request, validator);

            var dbContext = _userRepository.GetDbContext();

            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            var emailExisted = await _userRepository.GetByCondition(x => x.Email == request.Email, x => x.UserVerifications);

            if (emailExisted is not null)
            {
                var verifications = emailExisted.UserVerifications.Where(x => x.IsUsed == false);

                if (!emailExisted.IsVerified && verifications.Any())
                {
                    _logger.LogWarning("Intento de crear una cuenta con un correo pendiente de verificación: {Email}", request.Email);
                    throw new ConflictException("Este correo ya está registrado pero no ha sido verificado. Revisa tu bandeja o solicita un nuevo código.");
                }
                else
                {
                    _logger.LogWarning("Intento de crear usuario con un email ya existente y verificado: {Email}", request.Email);
                    throw new AlreadyExistsException("Ya existe una cuenta con este correo. Por favor, usa uno diferente.");
                }
            }

            var identificationNumberExisted = await _userRepository.GetByCondition(x => x.IdentificationNumber == request.IdentificationNumber);

            if (identificationNumberExisted is not null)
            {
                _logger.LogWarning("Se intentó registrar un usuario con un número de identificación ya registrado: {IdentificationNumber}", request.IdentificationNumber);
                throw new AlreadyExistsException("Número de identificación ya registrado.");
            }

            var entityMapped = _mapper.Map<UsersEntity>(request);
            entityMapped.PasswordHash = _passwordHasherService.HashPassword(request.Password);
            entityMapped.CreatedAt = DateTime.UtcNow;

            var codeValidation = await _codeGeneratorService.GenerateUniqueCode();

            var entity = new UserVerificationsEntity()
            {
                CodeValidation = codeValidation,
                CodeValidationExpiration = DateTime.UtcNow.AddMinutes(15),
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var userCreated = await _userRepository.Create(entityMapped);

                var response = _mapper.Map<UserResponse>(userCreated);

                entity.UserId = response.UserId;
                await _userVerificationRepository.Create(entity);

                await _emailService.SendEmail(
                    request.Email,
                    "Verificación de cuenta",
                    "..\\MyApp.Shared\\TemplateEmails\\VerificationTemplate.cshtml",
                    new Dictionary<string, string>
                    {
                        { "FirstName", request.FirstName },
                        { "CodeValidation", codeValidation },
                    });

                await transaction.CommitAsync();
                _logger.LogInformation("Usuario creado exitosamente con ID: {UserId}", response.UserId);

                return response;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Ocurrió un error durante la creación del usuario. Se realizó rollback.");
                throw;
            }
        }
    }
}
