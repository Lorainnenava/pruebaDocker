using System.Linq.Expressions;

namespace MyApp.Application.Interfaces.UseCases.Common
{
    public interface IGenericGetAllUseCase<TEntity, TResponse>
        where TEntity : class
        where TResponse : class
    {
        Task<IEnumerable<TResponse>> Execute(Expression<Func<TEntity, bool>>? condition = null, params Expression<Func<TEntity, object>>[] includes);
    }
}
