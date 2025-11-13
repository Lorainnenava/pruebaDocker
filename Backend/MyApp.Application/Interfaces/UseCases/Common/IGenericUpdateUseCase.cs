using System.Linq.Expressions;

namespace MyApp.Application.Interfaces.UseCases.Common
{
    public interface IGenericUpdateUseCase<TEntity, TRequest, TResponse>
        where TEntity : class
        where TRequest : class
        where TResponse : class
    {
        Task<TResponse> Execute(
            Expression<Func<TEntity, bool>> entityToUpdateCondition,
            Expression<Func<TEntity, bool>> isDuplicateNameWithDifferentIdCondition,
            TRequest request
        );
    }
}
