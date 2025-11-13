using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Application.Interfaces.Infrastructure;
using MyApp.Application.UseCases.RefreshTokens;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Tests.Mocks;
using System.Linq.Expressions;
using System.Security.Claims;

namespace MyApp.Tests.Application.RefreshTokens
{
    public class RefreshTokenServiceTests
    {
        private readonly Mock<IGenericRepository<RefreshTokensEntity>> _refreshTokenRepoMock;
        private readonly Mock<IGenericRepository<UserSessionsEntity>> _userSessionsRepoMock;
        private readonly Mock<IJwtHandler> _jwtHandlerMock;
        private readonly RefreshTokenUseCase _service;
        private readonly Mock<ILogger<RefreshTokenUseCase>> _loggerMock;

        public RefreshTokenServiceTests()
        {
            _refreshTokenRepoMock = new Mock<IGenericRepository<RefreshTokensEntity>>();
            _userSessionsRepoMock = new Mock<IGenericRepository<UserSessionsEntity>>();
            _jwtHandlerMock = new Mock<IJwtHandler>();
            _loggerMock = new Mock<ILogger<RefreshTokenUseCase>>();

            _service = new RefreshTokenUseCase(
                _refreshTokenRepoMock.Object,
                _userSessionsRepoMock.Object,
                _jwtHandlerMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Execute_WithValidRefreshToken_ReturnsNewAccessToken()
        {
            var refreshToken = MockRefreshToken.MockRefreshTokenEntity();

            _refreshTokenRepoMock
                .Setup(r => r.GetByCondition(
                    It.IsAny<Expression<Func<RefreshTokensEntity, bool>>>(),
                    It.IsAny<Expression<Func<RefreshTokensEntity, object>>[]>()))
                .ReturnsAsync(refreshToken);

            _jwtHandlerMock
                .Setup(j => j.GenerateAccessToken(It.IsAny<List<Claim>>()))
                .Returns("newAccessToken");

            var result = await _service.Execute("myRefreshToken");

            Assert.Equal("newAccessToken", result.AccessToken);
            Assert.Equal("myRefreshToken", result.RefreshToken);
        }

        [Fact]
        public async Task Execute_WithExpiredRefreshToken_DeletesSessionAndThrowsException()
        {
            var refreshToken = MockRefreshToken.MockRefreshTokenEntityExpired();
            var userSessionResponse = MockRefreshToken.MockUserSessionsEntityActiveFalse();
            var refreshTokenResponse = MockRefreshToken.MockRefreshTokenEntityExpiredWithActiveFalse();

            _refreshTokenRepoMock
                .Setup(r => r.GetByCondition(
                    It.IsAny<Expression<Func<RefreshTokensEntity, bool>>>(),
                    It.IsAny<Expression<Func<RefreshTokensEntity, object>>[]>()))
                .ReturnsAsync(refreshToken);

            _userSessionsRepoMock
                .Setup(r => r.Update(It.IsAny<UserSessionsEntity>()))
                .ReturnsAsync(userSessionResponse);

            _refreshTokenRepoMock
                .Setup(r => r.Update(It.IsAny<RefreshTokensEntity>()))
                .ReturnsAsync(refreshTokenResponse);

            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.Execute("expiredToken"));
            Assert.Contains("Tu sesión ha expirado. Por favor, inicia sesión nuevamente.", ex.Message);
        }
    }
}
