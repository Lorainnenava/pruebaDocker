using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Application.UseCases.UserVerifications;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using MyApp.Tests.Mocks;
using System.Linq.Expressions;

namespace MyApp.Tests.Application.UserVerifications
{
    public class UserValidateUseCaseTests
    {
        private readonly Mock<IGenericRepository<UsersEntity>> _userRepositoryMock;
        private readonly Mock<IGenericRepository<UserVerificationsEntity>> _userVerificationRepositoryMock;
        private readonly UserValidateUseCase _useCase;
        private readonly Mock<ILogger<UserValidateUseCase>> _loggerMock;

        public UserValidateUseCaseTests()
        {
            _userRepositoryMock = new Mock<IGenericRepository<UsersEntity>>();
            _loggerMock = new Mock<ILogger<UserValidateUseCase>>();
            _userVerificationRepositoryMock = new Mock<IGenericRepository<UserVerificationsEntity>>();

            _useCase = new UserValidateUseCase(_userVerificationRepositoryMock.Object,_userRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Execute_ShouldValidateUser_WhenCodeAndEmailExist()
        {
            var userEntity = MockUserVerifications.MockUserVerificationEntity();
            var response = MockUser.MockOneUserEntityUpdated();
            var request = MockUser.MockUserValidateRequest();

            _userVerificationRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<UserVerificationsEntity, bool>>>()))
                .ReturnsAsync(userEntity);

            _userRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>()))
                .ReturnsAsync(MockUser.MockOneUserEntity());

            _userRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<UsersEntity>()))
                .ReturnsAsync(response);

            _userVerificationRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<UserVerificationsEntity>()))
                .ReturnsAsync(userEntity);

            var result = await _useCase.Execute(request);

            Assert.True(result);
        }

        [Fact]
        public async Task Execute_ShouldThrowNotFoundException_WhenCodeAndEmailDoNotExist()
        {
            var request = MockUser.MockUserValidateRequest();

            _userRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>()))
                .ReturnsAsync((UsersEntity)null!);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _useCase.Execute(request));
            Assert.Equal("El código ingresado es incorrecto.", exception.Message);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenRequestIsInvalid()
        {
            var userRequest = MockUser.MockUserValidateInvalidCredentials();

            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _useCase.Execute(userRequest));
        }
    }
}
