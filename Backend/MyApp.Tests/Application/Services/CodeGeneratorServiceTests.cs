using Moq;
using MyApp.Application.Services;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;

namespace MyApp.Tests.Application.Services
{
    public class CodeGeneratorServiceTests
    {
        private readonly Mock<IGenericRepository<UserVerificationsEntity>> _userVerificationsRepositoryMock;
        private readonly Mock<IGenericRepository<UserPasswordResetsEntity>> _userPasswordResetRepositoryMock;
        private readonly CodeGeneratorService _service;

        public CodeGeneratorServiceTests()
        {
            _userVerificationsRepositoryMock = new Mock<IGenericRepository<UserVerificationsEntity>>();
            _userPasswordResetRepositoryMock = new Mock<IGenericRepository<UserPasswordResetsEntity>>();
            _service = new CodeGeneratorService(_userVerificationsRepositoryMock.Object, _userPasswordResetRepositoryMock.Object);
        }

        [Fact]
        public async Task GenerateUniqueCode_ShouldReturnUniqueCode()
        {
            _userVerificationsRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<UserVerificationsEntity, bool>>>()))
                .ReturnsAsync((UserVerificationsEntity)null!);

            string code = await _service.GenerateUniqueCode();

            Assert.NotNull(code);
            Assert.Matches(@"^\d{5}$", code);
        }

        [Fact]
        public async Task GenerateUniqueCode_ShouldRetryIfCodeAlreadyExists()
        {
            var existingUser = new UserVerificationsEntity { CodeValidation = "12345" };

            _userVerificationsRepositoryMock
                .SetupSequence(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<UserVerificationsEntity, bool>>>()))
                .ReturnsAsync(existingUser)
                .ReturnsAsync((UserVerificationsEntity)null!);

            string code = await _service.GenerateUniqueCode();

            Assert.NotNull(code);
            Assert.Matches(@"^\d{5}$", code);
            _userVerificationsRepositoryMock.Verify(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<UserVerificationsEntity, bool>>>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GenerateCodeByResetPassword_ShouldReturnUniqueCode()
        {
            _userPasswordResetRepositoryMock
                .Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<UserPasswordResetsEntity, bool>>>()))
                .ReturnsAsync((UserPasswordResetsEntity)null!);

            string code = await _service.GenerateCodeByResetPassword();

            Assert.NotNull(code);
            Assert.Matches(@"^\d{5}$", code);
        }

        [Fact]
        public async Task GenerateCodeByResetPassword_ShouldRetryIfCodeAlreadyExists()
        {
            var existingUser = new UserPasswordResetsEntity { ResetPasswordCode = "12345" };

            _userPasswordResetRepositoryMock
                .SetupSequence(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<UserPasswordResetsEntity, bool>>>()))
                .ReturnsAsync(existingUser)
                .ReturnsAsync((UserPasswordResetsEntity)null!);

            string code = await _service.GenerateCodeByResetPassword();

            Assert.NotNull(code);
            Assert.Matches(@"^\d{5}$", code);
            _userPasswordResetRepositoryMock.Verify(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<UserPasswordResetsEntity, bool>>>()), Times.Exactly(2));
        }
    }
}
