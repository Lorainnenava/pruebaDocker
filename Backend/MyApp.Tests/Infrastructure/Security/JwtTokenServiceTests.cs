using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyApp.Application.DTOs.Common;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Infrastructure.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyApp.Tests.Infrastructure.Security
{
    public class JwtTokenServiceTests
    {
        [Fact]
        public void GenerateAccessToken_ShouldReturnValidJwtToken()
        {
            var jwtSettings = Options.Create(new JwtSettings
            {
                SecretKey = "m1Cl4v3D3SeguridadCon32Caracteres!!",
                Issuer = "MyApp",
                Audience = "MyAppUsers",
                AccessTokenExpirationMinutes = 60
            });

            var loggerMock = new Mock<ILogger<JwtTokenService>>();
            var refreshTokenRepoMock = new Mock<IGenericRepository<RefreshTokensEntity>>();

            var service = new JwtTokenService(
                loggerMock.Object,
                jwtSettings,
                refreshTokenRepoMock.Object
            );

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "test@example.com")
            };

            var token = service.GenerateAccessToken(claims);

            Assert.False(string.IsNullOrWhiteSpace(token));

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == "1");
            Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Email && c.Value == "test@example.com");
        }

        [Fact]
        public async Task GenerateRefreshToken_ShouldReturnValidRefreshToken()
        {
            var jwtSettings = Options.Create(new JwtSettings
            {
                RefreshTokenExpirationHours = 7
            });

            var loggerMock = new Mock<ILogger<JwtTokenService>>();
            var refreshTokenRepoMock = new Mock<IGenericRepository<RefreshTokensEntity>>();

            // Simula que ningún token generado existe aún (para evitar bucle infinito)
            refreshTokenRepoMock
                .Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<RefreshTokensEntity, bool>>>()))
                .ReturnsAsync((RefreshTokensEntity)null!);

            var service = new JwtTokenService(
                loggerMock.Object,
                jwtSettings,
                refreshTokenRepoMock.Object
            );

            var refreshToken = await service.GenerateRefreshToken();

            Assert.NotNull(refreshToken);
            Assert.True(refreshToken.IsActive);
            Assert.False(string.IsNullOrWhiteSpace(refreshToken.Token));
            Assert.InRange(refreshToken.TokenExpirationDate, DateTime.UtcNow.AddDays(6.9), DateTime.UtcNow.AddDays(7.1));
        }

        [Fact]
        public void GenerateAccessToken_ShouldThrow_WhenSecretKeyIsMissing()
        {
            var jwtSettings = Options.Create(new JwtSettings
            {
                SecretKey = "",
                Issuer = "MyApp",
                Audience = "MyAppUsers",
                AccessTokenExpirationMinutes = 60
            });

            var loggerMock = new Mock<ILogger<JwtTokenService>>();
            var refreshTokenRepoMock = new Mock<IGenericRepository<RefreshTokensEntity>>();

            var service = new JwtTokenService(
                loggerMock.Object,
                jwtSettings,
                refreshTokenRepoMock.Object
            );

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test") };

            Assert.ThrowsAny<Exception>(() => service.GenerateAccessToken(claims));
        }
    }
}
