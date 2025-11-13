using AutoMapper;
using Microsoft.Extensions.Logging;
using MyApp.Application.Interfaces.UseCases.Common;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using System.Linq.Expressions;

namespace MyApp.Application.UseCases.Common
{
    public class GenericGetOneUseCase<TEntity, TResponse> : IGenericGetOneUseCase<TEntity, TResponse>
        where TEntity : class
        where TResponse : class
    {
        private readonly IGenericRepository<TEntity> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GenericGetOneUseCase<TEntity, TResponse>> _logger;

        public GenericGetOneUseCase(
            IGenericRepository<TEntity> repository,
            IMapper mapper,
            ILogger<GenericGetOneUseCase<TEntity, TResponse>> logger)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TResponse> Execute(Expression<Func<TEntity, bool>> condition)
        {
            _logger.LogInformation("Iniciando búsqueda de un registro de tipo {EntityName}.", typeof(TEntity).Name);

            TEntity? searchEntity = await _repository.GetByCondition(condition);

            if (searchEntity is null)
            {
                _logger.LogWarning("No se encontró ningún registro de tipo {EntityName} con la condición especificada.", typeof(TEntity).Name);
                throw new NotFoundException("El registro que estas buscando no existe o ha sido eliminado.");
            }

            var response = _mapper.Map<TResponse>(searchEntity);

            _logger.LogInformation("Registro de tipo {EntityName} obtenido correctamente.", typeof(TEntity).Name);

            return response;
        }
    }
}
