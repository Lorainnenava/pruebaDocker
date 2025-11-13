using AutoMapper;
using Microsoft.Extensions.Logging;
using MyApp.Application.Interfaces.UseCases.Common;
using MyApp.Domain.Interfaces.Infrastructure;
using System.Linq.Expressions;

namespace MyApp.Application.UseCases.Common
{
    public class GenericGetAllUseCase<TEntity, TResponse> : IGenericGetAllUseCase<TEntity, TResponse>
        where TEntity : class
        where TResponse : class
    {
        private readonly IGenericRepository<TEntity> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GenericGetAllUseCase<TEntity, TResponse>> _logger;

        public GenericGetAllUseCase(
            IGenericRepository<TEntity> repository,
            IMapper mapper,
            ILogger<GenericGetAllUseCase<TEntity, TResponse>> logger)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TResponse>> Execute(Expression<Func<TEntity, bool>>? condition = null, params Expression<Func<TEntity, object>>[] includes)
        {
            _logger.LogInformation("Iniciando la obtención para registros de tipo {EntityName}.", typeof(TEntity).Name);

            var searchItems = await _repository.GetAll(condition, includes);

            var response = _mapper.Map<IEnumerable<TResponse>>(searchItems);

            _logger.LogInformation("Se obtuvieron {Count} registros de tipo {EntityName}.", response.Count(), typeof(TEntity).Name);

            return response;
        }
    }
}
