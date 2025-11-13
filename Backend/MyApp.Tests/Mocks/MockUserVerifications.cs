using MyApp.Domain.Entities;

namespace MyApp.Tests.Mocks
{
    public class MockUserVerifications
    {
        public static UserVerificationsEntity MockUserVerificationEntity()
        {
            return new UserVerificationsEntity
            {
                UserVerificationId = 1,
                UserId = 1,
                CodeValidation = "56789",
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CodeValidationExpiration = DateTime.UtcNow.AddMinutes(15)
            };
        }

        public static ICollection<UserVerificationsEntity> MockUserVerificationsEntity()
        {
            return [
                new UserVerificationsEntity
                {
                    UserVerificationId = 1,
                    UserId = 1,
                    CodeValidation = "56789",
                    IsUsed = false,
                    CodeValidationExpiration = DateTime.UtcNow.AddMinutes(15),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new UserVerificationsEntity
                {
                    UserVerificationId = 2,
                    UserId = 1,
                    CodeValidation = "56739",
                    CodeValidationExpiration = DateTime.UtcNow.AddMinutes(15),
                    IsUsed = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
            ];
        }
    }
}
