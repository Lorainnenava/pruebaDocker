using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Infrastructure.Context;
using System.Linq.Expressions;

namespace MyApp.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DbContext GetDbContext()
        {
            return _dbContext;
        }

        public async Task<T> Create(T request)
        {
            var createEntity = _dbContext.Set<T>().Add(request);
            await _dbContext.SaveChangesAsync();
            return createEntity.Entity;
        }

        public async Task<bool> Delete(Expression<Func<T, bool>> condition)
        {
            var searchEntity = await _dbContext.Set<T>().FirstOrDefaultAsync(condition);

            if (searchEntity is null)
            {
                return false;
            }

            _dbContext.Set<T>().Remove(searchEntity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? condition = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            if (condition is not null)
            {
                query = query.Where(condition);
            }
            if (includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            ;

            return await query.ToListAsync();
        }

        public async Task<T?> GetByCondition(Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (includes?.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            ;

            return await query.FirstOrDefaultAsync(condition);
        }

        public async Task<T> Update(T entityToUpdate)
        {
            _dbContext.Set<T>().Update(entityToUpdate);

            await _dbContext.SaveChangesAsync();

            return entityToUpdate;
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> Pagination(int currentPage, int pageSize,
            Expression<Func<T, bool>>? condition = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            if (condition is not null)
            {
                query = query.Where(condition);
            }

            if (includes?.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (currentPage < 1) currentPage = 1;
            if (pageSize <= 0) pageSize = 10;

            int totalCount = await query.CountAsync();
            var items = await query
                .Skip(pageSize * (currentPage - 1))
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
