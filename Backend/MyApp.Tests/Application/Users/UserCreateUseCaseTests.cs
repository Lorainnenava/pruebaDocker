using AutoMapper;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Application.DTOs.Users;
using MyApp.Application.Interfaces.Infrastructure;
using MyApp.Application.Interfaces.Services;
using MyApp.Application.UseCases.Users;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using MyApp.Tests.Mocks;
using System.Linq.Expressions;

namespace MyApp.Tests.Application.Users
{
    public class UserCreateUseCaseTests
    {
        public readonly IMapper _mapper;
        private readonly Mock<IEmailService> _emailService;
        private readonly UserCreateUseCase _useCase;
        private readonly Mock<ILogger<UserCreateUseCase>> _loggerMock;
        private readonly Mock<ICodeGeneratorService> _codeGeneratorServiceMock;
        private readonly Mock<IPasswordHasherService> _passwordHasherServiceMock;
        public readonly Mock<IGenericRepository<UsersEntity>> _userRepositoryMock;
        private readonly Mock<IGenericRepository<UserVerificationsEntity>> _userVerificationsRepositoryMock;

        public UserCreateUseCaseTests()
        {
            _userRepositoryMock = new Mock<IGenericRepository<UsersEntity>>();
            _loggerMock = new Mock<ILogger<UserCreateUseCase>>();
            _codeGeneratorServiceMock = new Mock<ICodeGeneratorService>();
            _passwordHasherServiceMock = new Mock<IPasswordHasherService>();
            _emailService = new Mock<IEmailService>();
            _userVerificationsRepositoryMock = new Mock<IGenericRepository<UserVerificationsEntity>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserCreateRequest, UsersEntity>();
                cfg.CreateMap<UsersEntity, UserResponse>();
            });
            _mapper = config.CreateMapper();

            _useCase = new UserCreateUseCase(
                _mapper,
                _emailService.Object,
                _loggerMock.Object,
                _codeGeneratorServiceMock.Object,
                _passwordHasherServiceMock.Object,
                _userRepositoryMock.Object,
                _userVerificationsRepositoryMock.Object
                );
        }

        [Fact]
        public async Task Execute_ShouldCreateUserSuccessfully()
        {
            var userEntity = MockUser.MockOneUserEntityWithCodeValidation();
            var userRequest = MockUser.MockOneUserRequest();

            int callCount = 0;

            // Simular transacción
            var dbContextMock = new Mock<DbContext>();
            var dbFacadeMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();

            dbFacadeMock
                .Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            dbContextMock
                .Setup(db => db.Database)
                .Returns(dbFacadeMock.Object);

            _userRepositoryMock
                .Setup(repo => repo.GetDbContext())
                .Returns(dbContextMock.Object);

            _userRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>(), It.IsAny<Expression<Func<UsersEntity, object>>>()))
                .ReturnsAsync(() =>
                {
                    callCount++;
                    return callCount switch
                    {
                        1 => (UsersEntity)null!,
                        2 => (UsersEntity)null!,
                        _ => (UsersEntity)null!,
                    };
                });

            _passwordHasherServiceMock
                .Setup(service => service.HashPassword(userRequest.Password))
                .Returns("hashed_password_placeholder");

            _codeGeneratorServiceMock
                .Setup(service => service.GenerateUniqueCode())
                .ReturnsAsync("CODE12");

            _userVerificationsRepositoryMock
                .Setup(_userVerificationsRepositoryMock => _userVerificationsRepositoryMock.Create(It.IsAny<UserVerificationsEntity>()));

            _userRepositoryMock
                .Setup(x => x.Create(It.IsAny<UsersEntity>()))
                .ReturnsAsync(userEntity);

            var result = await _useCase.Execute(userRequest);

            Assert.NotNull(result);
            Assert.Equal(userEntity.UserId, result.UserId);
            Assert.Equal(userEntity.Email, result.Email);
        }

        [Fact]
        public async Task Execute_ShouldThrowConflictException_WhenEmailAlreadyExists()
        {
            var userEntity = MockUser.MockOneUserEntity();
            var userRequest = MockUser.MockOneUserRequest();

            // Simular transacción
            var dbContextMock = new Mock<DbContext>();
            var dbFacadeMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();

            dbFacadeMock
                .Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            dbContextMock
                .Setup(db => db.Database)
                .Returns(dbFacadeMock.Object);

            _userRepositoryMock
                .Setup(repo => repo.GetDbContext())
                .Returns(dbContextMock.Object);

            _userRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>(), It.IsAny<Expression<Func<UsersEntity, object>>>()))
                .ReturnsAsync(userEntity);

            _userVerificationsRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<UserVerificationsEntity, bool>>>()))
                .ReturnsAsync((UserVerificationsEntity)null!);

            var exception = await Assert.ThrowsAsync<AlreadyExistsException>(() => _useCase.Execute(userRequest));

            Assert.Equal("Ya existe una cuenta con este correo. Por favor, usa uno diferente.", exception.Message);
        }

        [Fact]
        public async Task Execute_ShouldThrowConflictException_WhenEmailAlreadyExistsAndHaveUserVerificationsAndIsVerifiedIsFalse()
        {
            var userEntity = MockUser.MockOneUserEntity();
            userEntity.IsVerified = false;
            var userRequest = MockUser.MockOneUserRequest();
            var userVerifications = MockUserVerifications.MockUserVerificationEntity();

            // Simular transacción
            var dbContextMock = new Mock<DbContext>();
            var dbFacadeMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();

            dbFacadeMock
                .Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            dbContextMock
                .Setup(db => db.Database)
                .Returns(dbFacadeMock.Object);

            _userRepositoryMock
                .Setup(repo => repo.GetDbContext())
                .Returns(dbContextMock.Object);

            _userRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>(), It.IsAny<Expression<Func<UsersEntity, object>>>()))
                .ReturnsAsync(userEntity);

            _userVerificationsRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<UserVerificationsEntity, bool>>>()))
                .ReturnsAsync(userVerifications);

            var exception = await Assert.ThrowsAsync<ConflictException>(() => _useCase.Execute(userRequest));

            Assert.Equal("Este correo ya está registrado pero no ha sido verificado. Revisa tu bandeja o solicita un nuevo código.", exception.Message);
        }

        [Fact]
        public async Task Execute_ShouldThrowAlreadyException_WhenIdentificationNumberExisted()
        {
            var userEntity = MockUser.MockOneUserEntity();
            var userRequest = MockUser.MockOneUserRequest();

            int callCount = 0;

            // Simular transacción
            var dbContextMock = new Mock<DbContext>();
            var dbFacadeMock = new Mock<DatabaseFacade>(dbContextMock.Object);
            var transactionMock = new Mock<IDbContextTransaction>();

            dbFacadeMock
                .Setup(db => db.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionMock.Object);

            dbContextMock
                .Setup(db => db.Database)
                .Returns(dbFacadeMock.Object);

            _userRepositoryMock
                .Setup(repo => repo.GetDbContext())
                .Returns(dbContextMock.Object);

            _userRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>(), It.IsAny<Expression<Func<UsersEntity, object>>>()))
                .ReturnsAsync(() =>
                {
                    callCount++;
                    return callCount switch
                    {
                        1 => null,
                        _ => null
                    };
                });

            // Mock para la segunda sobrecarga (sin include)
            _userRepositoryMock
                .Setup(repo => repo.GetByCondition(
                    It.IsAny<Expression<Func<UsersEntity, bool>>>()))
                .ReturnsAsync(() =>
                {
                    callCount++;
                    return callCount switch
                    {
                        2 => MockUser.MockOneUserEntity(),
                        _ => null
                    };
                });

            var exception = await Assert.ThrowsAsync<AlreadyExistsException>(() => _useCase.Execute(userRequest));
            Assert.Equal("Número de identificación ya registrado.", exception.Message);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenRequestIsInvalid()
        {
            var userRequest = MockUser.MockUserCreateInvalidCredentials();

            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _useCase.Execute(userRequest));
        }
    }
}
