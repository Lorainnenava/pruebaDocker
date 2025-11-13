namespace MyApp.Application.Interfaces.UseCases.Common
{
    public interface IGenericCreateUseCase<TEntity, TResponse>
        where TEntity : class
        where TResponse : class
    {
        Task<TResponse> Excecute(string Name);
    }
}
