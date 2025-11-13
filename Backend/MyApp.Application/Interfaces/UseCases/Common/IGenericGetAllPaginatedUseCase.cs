using MyApp.Shared.DTOs;

namespace MyApp.Application.Interfaces.UseCases.Common
{
    public interface IGenericGetAllPaginatedUseCase<TEntity, TResponse>
        where TEntity : class
        where TResponse : class
    {
        Task<PaginationResult<TResponse>> Execute(int page, int size);
    }
}
