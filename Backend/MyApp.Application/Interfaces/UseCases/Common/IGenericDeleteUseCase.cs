using System.Linq.Expressions;

namespace MyApp.Application.Interfaces.UseCases.Common
{
    public interface IGenericDeleteUseCase<TEntity> where TEntity : class
    {
        Task<bool> Execute(
            Expression<Func<TEntity, bool>> condition,
            Func<TEntity, bool>? validateDelete = null,
            params Expression<Func<TEntity, object>>[] includes
        );
    }
}
