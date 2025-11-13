using MyApp.Infrastructure.Services;

namespace MyApp.Tests.Infrastructure.Services
{
    public class PasswordHasherServiceTests
    {
        private readonly PasswordHasherService _hasher;

        public PasswordHasherServiceTests()
        {
            _hasher = new PasswordHasherService();
        }

        [Fact]
        public void HashPassword_ShouldGenerateDifferentHashes_ForSamePassword()
        {
            string password = "MySecurePassword123!";

            string hash1 = _hasher.HashPassword(password);
            string hash2 = _hasher.HashPassword(password);

            Assert.NotEqual(hash1, hash2); // Por el salt aleatorio, deben ser distintos
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordIsCorrect()
        {
            string password = "MySecurePassword123!";
            string hash = _hasher.HashPassword(password);

            bool result = _hasher.VerifyPassword(password, hash);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordIsIncorrect()
        {
            string correctPassword = "MySecurePassword123!";
            string wrongPassword = "WrongPassword!";
            string hash = _hasher.HashPassword(correctPassword);

            bool result = _hasher.VerifyPassword(wrongPassword, hash);

            Assert.False(result);
        }
    }
}
