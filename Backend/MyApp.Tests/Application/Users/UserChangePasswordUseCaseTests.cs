using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Application.Interfaces.Infrastructure;
using MyApp.Application.UseCases.Users;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using MyApp.Tests.Mocks;
using System.Linq.Expressions;

namespace MyApp.Tests.Application.Users
{
    public class UserChangePasswordUseCaseTests
    {
        private readonly Mock<IGenericRepository<UsersEntity>> _userRepositoryMock;
        private readonly UserChangePasswordUseCase _useCase;
        private readonly Mock<ILogger<UserChangePasswordUseCase>> _loggerMock;
        public readonly Mock<IPasswordHasherService> _passwordHasherServiceMock;

        public UserChangePasswordUseCaseTests()
        {
            _userRepositoryMock = new Mock<IGenericRepository<UsersEntity>>();
            _loggerMock = new Mock<ILogger<UserChangePasswordUseCase>>();
            _passwordHasherServiceMock = new Mock<IPasswordHasherService>();

            _useCase = new UserChangePasswordUseCase(_userRepositoryMock.Object, _loggerMock.Object, _passwordHasherServiceMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldUpdatePassword_WhenUserExistsAndCurrentPasswordIsCorrect()
        {
            var userEntity = MockUser.MockOneUserEntity();
            var request = MockUser.UserChangePasswordRequestValid();

            _userRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>()))
                .ReturnsAsync(userEntity);

            int callCount = 0;

            _passwordHasherServiceMock
                .Setup(repo => repo.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() =>
                {
                    callCount++;
                    return callCount switch
                    {
                        1 => true,
                        2 => false,
                        _ => true,
                    };
                });

            _passwordHasherServiceMock
                .Setup(service => service.HashPassword(request.NewPassword))
                .Returns("hashed_password_placeholder");

            var result = await _useCase.Execute(1, request);

            Assert.True(result);
            _userRepositoryMock.Verify(repo => repo.Update(It.Is<UsersEntity>(u => u.UserId == 1)), Times.Once);
        }

        [Fact]
        public async Task Execute_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            var request = MockUser.UserChangePasswordRequestValid();

            _userRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>()))
                .ReturnsAsync((UsersEntity)null!);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _useCase.Execute(1, request));

            Assert.Equal("Este usuario no existe o ha sido eliminado.", exception.Message);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenCurrentPasswordIsIncorrect()
        {
            var userEntity = MockUser.MockOneUserEntity();
            var request = MockUser.UserChangePasswordRequestValid();

            _userRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>()))
                .ReturnsAsync(userEntity);

            _passwordHasherServiceMock
                .Setup(repo => repo.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.Execute(1, request));

            Assert.Equal("La contraseña actual es incorrecta.", exception.Message);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenNewPasswordIsSameCurrentPassword()
        {
            var userEntity = MockUser.MockOneUserEntity();
            var request = MockUser.UserChangePasswordRequestValid();

            _userRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>()))
                .ReturnsAsync(userEntity);

            int callCount = 0;

            _passwordHasherServiceMock
                .Setup(repo => repo.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() =>
                {
                    callCount++;
                    return callCount switch
                    {
                        1 => true,
                        2 => true,
                        _ => false,
                    };
                });

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.Execute(1, request));

            Assert.Equal("La nueva contraseña no puede ser igual a la contraseña actual.", exception.Message);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenRequestIsInvalid()
        {
            var userRequest = MockUser.UserChangePasswordRequestInvalid();

            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _useCase.Execute(1, userRequest));
        }
    }
}
