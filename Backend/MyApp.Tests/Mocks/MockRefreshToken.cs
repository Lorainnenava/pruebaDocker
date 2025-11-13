using MyApp.Domain.Entities;

namespace MyApp.Tests.Mocks
{
    public class MockRefreshToken
    {
        public static RefreshTokensEntity MockRefreshTokenEntity()
        {
            return new RefreshTokensEntity
            {
                RefreshTokenId = 1,
                UserSessionId = 101,
                Token = "myRefreshToken",
                TokenExpirationDate = DateTime.UtcNow.AddDays(7),
                IsActive = true,
                UserSession = new UserSessionsEntity
                {
                    UserSessionId = 101,
                    UserId = 202,
                    IpAddress = "192.168.0.123",
                    IsRevoked = false,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow,
                    User = new UsersEntity
                    {
                        UserId = 202,
                        RoleId = 1,
                        Role = new RolesEntity
                        {
                            RoleId = 1,
                            Name = "Admin"
                        }
                    }
                }
            };
        }

        public static RefreshTokensEntity MockRefreshTokenEntityWithoutUserSession()
        {
            return new RefreshTokensEntity
            {
                RefreshTokenId = 1,
                UserSessionId = 101,
                Token = "myRefreshToken",
                TokenExpirationDate = DateTime.UtcNow.AddDays(7),
                IsActive = true,
                UserSession = null
            };
        }

        public static RefreshTokensEntity MockRefreshTokenEntityExpired()
        {
            return new RefreshTokensEntity
            {
                RefreshTokenId = 1,
                UserSessionId = 1,
                Token = "expiredToken",
                IsActive = true,
                TokenExpirationDate = DateTime.UtcNow.AddMinutes(-1),
                UserSession = new UserSessionsEntity
                {
                    UserSessionId = 101,
                    UserId = 202,
                    IpAddress = "192.168.0.123",
                    IsRevoked = false,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow,
                    User = new UsersEntity
                    {
                        UserId = 202,
                        RoleId = 1,
                        Role = new RolesEntity
                        {
                            RoleId = 1,
                            Name = "Admin"
                        }
                    }
                }
            };
        }

        public static RefreshTokensEntity MockRefreshTokenEntityExpiredWithActiveFalse()
        {
            return new RefreshTokensEntity
            {
                RefreshTokenId = 1,
                UserSessionId = 1,
                Token = "expiredToken",
                IsActive = false,
                TokenExpirationDate = DateTime.UtcNow.AddMinutes(-1),
            };
        }

        public static UserSessionsEntity MockUserSessionsEntityActiveFalse()
        {
            return new UserSessionsEntity
            {
                UserSessionId = 101,
                UserId = 202,
                IpAddress = "192.168.0.123",
                IsRevoked = true,
                ExpiresAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow,
            };
        }
    }
}
