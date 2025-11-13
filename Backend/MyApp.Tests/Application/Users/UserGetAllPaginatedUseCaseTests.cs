using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Application.UseCases.Users;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Tests.Mocks;
using System.Linq.Expressions;

namespace MyApp.Tests.Application.Users
{
    public class UserGetAllPaginatedUseCaseTests
    {
        private readonly Mock<IGenericRepository<UsersEntity>> _userRepositoryMock;
        private readonly Mock<ILogger<UserGetAllPaginatedUseCase>> _loggerMock;
        private readonly UserGetAllPaginatedUseCase _useCase;

        public UserGetAllPaginatedUseCaseTests()
        {
            _userRepositoryMock = new Mock<IGenericRepository<UsersEntity>>();
            _loggerMock = new Mock<ILogger<UserGetAllPaginatedUseCase>>();

            _useCase = new UserGetAllPaginatedUseCase(
                _userRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Execute_ShouldReturnPaginatedUsersSuccessfully()
        {
            int page = 1, size = 2;
            var usersEntity = MockUser.MockListUsersEntity();

            _userRepositoryMock
                .Setup(repo => repo.Pagination(
                    page, size,
                    null,
                    It.IsAny<Expression<Func<UsersEntity, object>>[]>()))
                .ReturnsAsync((usersEntity, usersEntity.Count));

            var result = await _useCase.Execute(page, size);

            Assert.NotNull(result);
            Assert.Equal(usersEntity.Count, result.RowsCount);
            Assert.Equal((int)Math.Ceiling((double)usersEntity.Count / size), result.PageCount);
            Assert.Equal(page, result.CurrentPage);
            Assert.Equal(size, result.PageSize);
            Assert.Equal(usersEntity.Count, result.Results.Count());
        }

        [Fact]
        public async Task Execute_ShouldReturnEmptyPaginationResult_WhenNoUsers()
        {
            int page = 1, size = 10;
            var emptyList = new List<UsersEntity>();

            _userRepositoryMock
                .Setup(repo => repo.Pagination(page, size, null, Array.Empty<Expression<Func<UsersEntity, object>>>()))
                .ReturnsAsync((emptyList, 0));

            var result = await _useCase.Execute(page, size);

            Assert.NotNull(result);
            Assert.Empty(result.Results);
            Assert.Equal(0, result.RowsCount);
            Assert.Equal(0, result.PageCount);
            Assert.Equal(page, result.CurrentPage);
            Assert.Equal(size, result.PageSize);
        }
    }
}
