using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MyApp.Domain.Interfaces.Infrastructure
{
    public interface IGenericRepository<T>
    {
        DbContext GetDbContext();
        Task<T> Create(T request);
        Task<T?> GetByCondition(Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? condition = null, params Expression<Func<T, object>>[] includes);
        Task<T> Update(T entityToUpdate);
        Task<bool> Delete(Expression<Func<T, bool>> condition);
        Task<(IEnumerable<T> Items, int TotalCount)> Pagination(int currentPage, int pageSize,
            Expression<Func<T, bool>>? condition = null, params Expression<Func<T, object>>[] includes);
    }
}