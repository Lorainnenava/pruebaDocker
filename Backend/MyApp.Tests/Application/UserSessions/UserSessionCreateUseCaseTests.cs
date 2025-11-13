using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Application.DTOs.UserSessions;
using MyApp.Application.Interfaces.Infrastructure;
using MyApp.Application.Interfaces.UseCases.UserSessions;
using MyApp.Application.UseCases.UserSessions;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using MyApp.Tests.Mocks;
using System.Linq.Expressions;
using System.Security.Claims;

namespace MyApp.Tests.Application.UserSessions
{
    public class UserSessionCreateUseCaseTests
    {
        private readonly Mock<IGenericRepository<UsersEntity>> _usersRepoMock;
        private readonly Mock<IGenericRepository<UserSessionsEntity>> _userSessionsRepoMock;
        private readonly Mock<IGenericRepository<RefreshTokensEntity>> _refreshTokensRepoMock;
        private readonly Mock<IUserSessionRevokedUseCase> _userSessionRevockedMock;
        private readonly Mock<IJwtHandler> _jwtHandlerMock;
        private readonly UserSessionCreateUseCase _useCase;
        private readonly Mock<IPasswordHasherService> _passwordHasherMock;
        private readonly Mock<ILogger<UserSessionCreateUseCase>> _loggerMock;

        public UserSessionCreateUseCaseTests()
        {
            _usersRepoMock = new Mock<IGenericRepository<UsersEntity>>();
            _userSessionsRepoMock = new Mock<IGenericRepository<UserSessionsEntity>>();
            _refreshTokensRepoMock = new Mock<IGenericRepository<RefreshTokensEntity>>();
            _jwtHandlerMock = new Mock<IJwtHandler>();
            _loggerMock = new Mock<ILogger<UserSessionCreateUseCase>>();
            _passwordHasherMock = new Mock<IPasswordHasherService>();
            _userSessionRevockedMock = new Mock<IUserSessionRevokedUseCase>();

            _useCase = new UserSessionCreateUseCase(
                _usersRepoMock.Object,
                _userSessionsRepoMock.Object,
                _jwtHandlerMock.Object,
                _refreshTokensRepoMock.Object,
                _passwordHasherMock.Object,
                _userSessionRevockedMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Execute_WithValidCredentials_ReturnsAccessAndRefreshToken()
        {
            var request = MockUserSession.MockUserSessionsRequestDto();
            var user = MockUserSession.MockUsersEntityCorrect();
            var createdSession = MockUserSession.MockUserSessionsEntity();
            var refreshToken = MockUserSession.MockRefreshTokensEntityTwo();
            var responseToken = MockUserSession.MockGenerateRefreshToken();

            _usersRepoMock
                .Setup(r => r.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>(), It.IsAny<Expression<Func<UsersEntity, object>>>()))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(service => service.VerifyPassword(request.Password, user.PasswordHash))
                .Returns(true);

            _jwtHandlerMock
                .Setup(j => j.GenerateAccessToken(It.IsAny<List<Claim>>()))
                .Returns("accessToken123");

            _jwtHandlerMock
                .Setup(j => j.GenerateRefreshToken())
                .ReturnsAsync(responseToken);


            _userSessionsRepoMock
                .Setup(r => r.Create(It.IsAny<UserSessionsEntity>()))
                .ReturnsAsync(createdSession);

            _refreshTokensRepoMock
                .Setup(r => r.Create(It.IsAny<RefreshTokensEntity>()))
                .ReturnsAsync(refreshToken);

            var result = await _useCase.Execute(request);

            Assert.Equal("accessToken123", result.AccessToken);
            Assert.Equal("refreshToken123", result.RefreshToken);
        }

        [Fact]
        public async Task Execute_WithInvalidCredentials_ThrowsException()
        {
            var request = MockUserSession.MockUserSessionsRequestDtoWrong();

            _usersRepoMock
                .Setup(r => r.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>()))
                .ReturnsAsync((UsersEntity)null!);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _useCase.Execute(request));
            Assert.Contains("Email o contraseña incorrectos. Por favor, intenta de nuevo.", ex.Message);
        }

        [Fact]
        public async Task Execute_WithInactiveUser_ThrowsNotFoundException()
        {
            var request = MockUserSession.MockUserSessionsRequestDto();
            var user = MockUserSession.MockUserSessionsEntityWithNotActive();

            _usersRepoMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>(), It.IsAny<Expression<Func<UsersEntity, object>>>()))
                .ReturnsAsync(user);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _useCase.Execute(request));
            Assert.Contains("Cuenta inactiva o no verificada.", ex.Message);
        }

        [Fact]
        public async Task Execute_WithInvalidPassword_ThrowsInvalidDataException()
        {
            var request = MockUserSession.MockUserSessionsRequestDtoWrong();
            var user = MockUserSession.MockUsersEntityCorrect();

            _usersRepoMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>()))
                .ReturnsAsync(user);

            _passwordHasherMock.Setup(p => p.VerifyPassword(request.Password, user.PasswordHash))
                .Returns(false);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _useCase.Execute(request));
            Assert.Contains("Email o contraseña incorrectos. Por favor, intenta de nuevo.", ex.Message);
        }

        [Fact]
        public async Task Execute_ShouldThrowValidationException_WhenRequestIsInvalid()
        {
            var userRequest = new UserSessionRequest
            {
                Email = "",
                Password = "123"
            };

            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _useCase.Execute(userRequest));
        }

        [Fact]
        public async Task Execute_WithActiveSession_ShouldRevokePreviousSession()
        {
            var request = MockUserSession.MockUserSessionsRequestDto();
            var user = MockUserSession.MockUsersEntityCorrect();
            var existingSession = MockUserSession.MockUserSessionsEntity();
            var refreshToken = MockUserSession.MockRefreshTokensEntityTwo();
            var newRefreshToken = MockUserSession.MockGenerateRefreshToken();

            _usersRepoMock
                .Setup(r => r.GetByCondition(It.IsAny<Expression<Func<UsersEntity, bool>>>(), It.IsAny<Expression<Func<UsersEntity, object>>>()))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(p => p.VerifyPassword(request.Password, user.PasswordHash))
                .Returns(true);

            _userSessionsRepoMock
                .Setup(r => r.GetByCondition(
                    It.IsAny<Expression<Func<UserSessionsEntity, bool>>>(),
                    It.IsAny<Expression<Func<UserSessionsEntity, object>>>()
                ))
                .ReturnsAsync(existingSession);

            _userSessionRevockedMock
                .Setup(r => r.Execute(existingSession.RefreshTokenEntity.Token))
                .ReturnsAsync(true)
                .Verifiable();

            _jwtHandlerMock
                .Setup(j => j.GenerateAccessToken(It.IsAny<List<Claim>>()))
                .Returns("accessToken123");

            _jwtHandlerMock
                .Setup(j => j.GenerateRefreshToken())
                .ReturnsAsync(newRefreshToken);

            _userSessionsRepoMock
                .Setup(r => r.Create(It.IsAny<UserSessionsEntity>()))
                .ReturnsAsync(MockUserSession.MockUserSessionsEntity());

            _refreshTokensRepoMock
                .Setup(r => r.Create(It.IsAny<RefreshTokensEntity>()))
                .ReturnsAsync(refreshToken);

            var result = await _useCase.Execute(request);

            _userSessionRevockedMock.Verify(r => r.Execute(existingSession.RefreshTokenEntity.Token), Times.Once);
            Assert.Equal("accessToken123", result.AccessToken);
            Assert.Equal("refreshToken123", result.RefreshToken);
        }

    }
}
