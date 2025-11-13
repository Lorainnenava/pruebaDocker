using AutoMapper;
using Microsoft.Extensions.Logging;
using MyApp.Application.Interfaces.UseCases.Common;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using MyApp.Shared.Interfaces;
using MyApp.Shared.Services;
using System.Linq.Expressions;

namespace MyApp.Application.UseCases.Common
{
    public class GenericUpdateUseCase<TEntity, TRequest, TResponse> : IGenericUpdateUseCase<TEntity, TRequest, TResponse>
        where TEntity : class, INameable
        where TResponse : class
        where TRequest : class, INameable, new()
    {
        private readonly IGenericRepository<TEntity> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GenericUpdateUseCase<TEntity, TRequest, TResponse>> _logger;

        public GenericUpdateUseCase(
            IGenericRepository<TEntity> repository,
            IMapper mapper,
            ILogger<GenericUpdateUseCase<TEntity, TRequest, TResponse>> logger)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TResponse> Execute(
            Expression<Func<TEntity, bool>> entityToUpdateCondition,
            Expression<Func<TEntity, bool>> isDuplicateNameWithDifferentIdCondition,
            TRequest request)
        {
            _logger.LogInformation("Iniciando la actualización de un registro de tipo {EntityName}.", typeof(TEntity).Name);

            var all = await _repository.GetAll();
            var entityToUpdateFunc = entityToUpdateCondition.Compile();
            var isDuplicateNameWithDifferentIdFunc = isDuplicateNameWithDifferentIdCondition.Compile();
            var entityToUpdate = all.FirstOrDefault(entityToUpdateFunc);

            if (entityToUpdate is null)
            {
                _logger.LogWarning("No se encontró ningún registro de tipo {EntityName} con la condición especificada.", typeof(TEntity).Name);
                throw new NotFoundException("El registro que estas buscando no existe o ha sido eliminado.");
            }

            var normalizedName = NormalizeService.Normalize(request.Name);

            var exists = all.FirstOrDefault(x => NormalizeService.Normalize(x.Name) == normalizedName && isDuplicateNameWithDifferentIdFunc(x));

            if (exists is not null)
            {
                _logger.LogWarning("Ya existe un registro con el nombre {Name}.", request.Name);
                throw new AlreadyExistsException("Ya existe un registro con este nombre.");
            }

            _mapper.Map(request, entityToUpdate);

            var updatedEntity = await _repository.Update(entityToUpdate);

            var response = _mapper.Map<TResponse>(updatedEntity);

            _logger.LogInformation("Registro de tipo {EntityName} actualizado correctamente.", typeof(TEntity).Name);

            return response;
        }
    }
}
