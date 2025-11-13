using System.Linq.Expressions;

namespace MyApp.Application.Interfaces.UseCases.Common
{
    public interface IGenericGetOneUseCase<TEntity, TResponse>
        where TEntity : class
        where TResponse : class
    {
        Task<TResponse> Execute(Expression<Func<TEntity, bool>> condition);
    }
}
