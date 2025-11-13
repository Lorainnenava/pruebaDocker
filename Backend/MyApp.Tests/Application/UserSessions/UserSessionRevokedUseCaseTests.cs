using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Application.UseCases.UserSessions;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using MyApp.Tests.Mocks;
using System.Linq.Expressions;

namespace MyApp.Tests.Application.UserSessions
{
    public class UserSessionRevokedUseCaseTests
    {
        private readonly Mock<IGenericRepository<RefreshTokensEntity>> _refreshTokenRepoMock;
        private readonly Mock<IGenericRepository<UserSessionsEntity>> _userSessionRepoMock;
        private readonly Mock<ILogger<UserSessionRevokedUseCase>> _loggerMock;
        private readonly UserSessionRevokedUseCase _useCase;

        public UserSessionRevokedUseCaseTests()
        {
            _refreshTokenRepoMock = new Mock<IGenericRepository<RefreshTokensEntity>>();
            _userSessionRepoMock = new Mock<IGenericRepository<UserSessionsEntity>>();
            _loggerMock = new Mock<ILogger<UserSessionRevokedUseCase>>();

            _useCase = new UserSessionRevokedUseCase(
                _refreshTokenRepoMock.Object,
                _userSessionRepoMock.Object,
                _loggerMock.Object
            );
        }
         
        [Fact]
        public async Task Execute_WithValidData_ReturnsTrue()
        {
            var refreshToken = MockRefreshToken.MockRefreshTokenEntity();

            _refreshTokenRepoMock
                .Setup(r => r.GetByCondition(
                    It.IsAny<Expression<Func<RefreshTokensEntity, bool>>>(),
                    It.IsAny<Expression<Func<RefreshTokensEntity, object>>[]>()))
                .ReturnsAsync(refreshToken);

            _refreshTokenRepoMock
                .Setup(r => r.Update(It.IsAny<RefreshTokensEntity>()))
                .ReturnsAsync(refreshToken);

            _userSessionRepoMock
                .Setup(r => r.Update(It.IsAny<UserSessionsEntity>()))
                .ReturnsAsync((UserSessionsEntity entity) => entity);

            var result = await _useCase.Execute("myRefreshToken");

            Assert.True(result);
        }

        [Fact]
        public async Task Execute_RefreshTokenNotFound_ThrowsException()
        {
            _refreshTokenRepoMock
                .Setup(r => r.GetByCondition(x => x.Token == It.IsAny<string>(), x => x.UserSession))
                .ReturnsAsync((RefreshTokensEntity)null!);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _useCase.Execute("invalid-token"));

            Assert.Contains("No existe ningun refresh token con este valor.", ex.Message);
        }

        [Fact]
        public async Task Execute_UserSessionNotFound_ThrowsException()
        {
            var response = MockRefreshToken.MockRefreshTokenEntityWithoutUserSession();

            _refreshTokenRepoMock
                .Setup(r => r.GetByCondition(It.IsAny<Expression<Func<RefreshTokensEntity, bool>>>(), x => x.UserSession))
                .ReturnsAsync(response);

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _useCase.Execute("myRefreshToken"));
            Assert.Contains("No existe ninguna sessión asociada a este refresh token.", ex.Message);
        }
    }
}
