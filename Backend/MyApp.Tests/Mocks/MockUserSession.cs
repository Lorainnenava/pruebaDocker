using MyApp.Application.DTOs.UserSessions;
using MyApp.Domain.Entities;

namespace MyApp.Tests.Mocks
{
    public class MockUserSession
    {
        public static UserSessionRequest MockUserSessionsRequestDto()
        {
            return new UserSessionRequest
            {
                Email = "user@example.com",
                Password = "123456",
                IpAddress = "127.0.0.1",
            };
        }

        public static UsersEntity MockUsersEntityCorrect()
        {
            return new UsersEntity
            {
                Email = "user@example.com",
                PasswordHash = "hashed_password_placeholder",
                UserId = 1,
                IsActive = true,
                IsVerified = true,
                RoleId = 1,
                Role = new RolesEntity
                {
                    RoleId = 1,
                    Name = "User"
                }
            };
        }

        public static UserSessionsEntity MockUserSessionsEntity()
        {
            return new UserSessionsEntity
            {
                UserSessionId = 99,
                UserId = 1,
                IpAddress = "127.0.0.1",
                RefreshTokenEntity = MockRefreshTokensEntity(),
            };
        }

        public static RefreshTokensEntity MockRefreshTokensEntity()
        {
            return new RefreshTokensEntity
            {
                RefreshTokenId = 1,
                UserSessionId = 99,
                IsActive = true,
                Token = "refreshToken123",
                TokenExpirationDate = DateTime.UtcNow.AddDays(7),
                UserSession = new UserSessionsEntity
                {
                    UserSessionId = 99,
                    UserId = 1,
                    IpAddress = "192.168.0.123",
                    IsRevoked = false,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow,
                }
            };
        }

        public static RefreshTokensEntity MockGenerateRefreshToken()
        {
            return new RefreshTokensEntity
            {
                IsActive = true,
                Token = "refreshToken123",
                TokenExpirationDate = DateTime.UtcNow.AddDays(7),
            };
        }

        public static RefreshTokensEntity MockRefreshTokensEntityTwo()
        {
            return new RefreshTokensEntity
            {
                RefreshTokenId = 1,
                UserSessionId = 99,
                IsActive = true,
                Token = "refreshToken123",
                TokenExpirationDate = DateTime.UtcNow.AddDays(7)
            };
        }

        public static UserSessionRequest MockUserSessionsRequestDtoWrong()
        {
            return new UserSessionRequest
            {
                Email = "fake@example.com",
                Password = "wrong",
                IpAddress = "127.0.0.1",
            };
        }

        public static UsersEntity MockUserSessionsEntityWithNotActive()
        {
            return new UsersEntity
            {
                Email = "user@example.com",
                PasswordHash = "hashed_correct",
                IsActive = false,
            };
        }
    }
}
