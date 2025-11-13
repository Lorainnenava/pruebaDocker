using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Context;
using MyApp.Infrastructure.Repositories;
using MyApp.Tests.Mocks;

namespace MyApp.Tests.Infrastructure.Repositories
{
    public class GenericRepositoryTests_SuccessCases
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly GenericRepository<UsersEntity> _genericRepository;
        private readonly List<UsersEntity> _fakeData;

        public GenericRepositoryTests_SuccessCases()
        {
            _dbContext = GetDbContext();
            _fakeData = MockUser.MockListUsersEntity();
            _dbContext.Users.AddRange(_fakeData);
            _dbContext.SaveChanges();
            _genericRepository = new GenericRepository<UsersEntity>(_dbContext);
        }

        private static ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Create_ShouldAddNewEntity_WhenValidDataProvided()
        {
            var entityToCreate = MockUser.MockOneUserEntityToCreate();

            var result = await _genericRepository.Create(entityToCreate);

            Assert.NotNull(result);
            Assert.Equal(3, _dbContext.Users.Count());
            Assert.Equal("usuario.prueba@example.com", result.Email);
            Assert.True(await _dbContext.Users.AnyAsync(p => p.IdentificationNumber == "1234567890"));
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity_WhenEntityExists()
        {
            bool result = await _genericRepository.Delete(x => x.UserId == 1);

            Assert.True(result);
            Assert.Equal(1, _dbContext.Users.Count());
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllEntities_WhenCalled()
        {
            IEnumerable<UsersEntity> result = await _genericRepository.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Collection(result,
                item => Assert.Equal("usuario.prueba@example.com", item.Email),
                item => Assert.Equal("23456789", item.IdentificationNumber)
            );
        }

        [Fact]
        public async Task Update_WithTwoParameters_ShouldUpdateFieldsCorrectly()
        {
            var updatedEntity = MockUser.MockOneUserEntityUpdated();

            var trackedEntity = await _dbContext.Users.FindAsync(updatedEntity.UserId);
            if (trackedEntity != null)
            {
                _dbContext.Entry(trackedEntity).State = EntityState.Detached;
            }

            var result = await _genericRepository.Update(updatedEntity);

            Assert.NotNull(result);
            Assert.Equal("+57 300 123 4685", result.Phone);
            Assert.Equal("Segundo apellido", result.SecondName);
            Assert.Equal("1234567890", result.IdentificationNumber);
        }


        [Fact]
        public async Task GetAllPaginated_ShouldReturnCorrectPage()
        {
            var (items, totalCount) = await _genericRepository.Pagination(1, 1);

            Assert.Single(items);
            Assert.Equal("usuario.prueba@example.com", items.First().Email);
            Assert.Equal(2, totalCount);
        }
    }
}