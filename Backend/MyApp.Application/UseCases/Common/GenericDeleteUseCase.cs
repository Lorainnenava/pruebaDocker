using Microsoft.Extensions.Logging;
using MyApp.Application.Interfaces.UseCases.Common;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using System.Linq.Expressions;

namespace MyApp.Application.UseCases.Common
{
    internal class GenericDeleteUseCase<TEntity> : IGenericDeleteUseCase<TEntity>
        where TEntity : class
    {
        private readonly IGenericRepository<TEntity> _repository;
        private readonly ILogger<GenericDeleteUseCase<TEntity>> _logger;
        public GenericDeleteUseCase(
            IGenericRepository<TEntity> repository,
            ILogger<GenericDeleteUseCase<TEntity>> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Execute(
            Expression<Func<TEntity, bool>> condition,
            Func<TEntity, bool>? validateDelete = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            _logger.LogInformation("Iniciando proceso de eliminación de una entidad del tipo {EntityName}.", typeof(TEntity).Name);

            var searchEntity = await _repository.GetByCondition(
                condition,
                includes
            );

            if (searchEntity is null)
            {
                _logger.LogWarning("No se encontró ningún registro de tipo {EntityName} con la condición especificada.", typeof(TEntity).Name);
                throw new NotFoundException("El registro que intenta eliminar no existe o ya fue eliminado.");
            }

            if (validateDelete is not null && !validateDelete(searchEntity))
            {
                _logger.LogWarning("No se puede eliminar la entidad del tipo {EntityName} porque está relacionada con otros registros.", typeof(TEntity).Name);
                throw new ConflictException($"No se puede eliminar este registro porque está relacionado con otros registros.");
            }

            bool deleted = await _repository.Delete(condition);

            _logger.LogInformation("La entidad del tipo {EntityName} fue eliminado exitosamente.", typeof(TEntity).Name);
            return deleted;
        }
    }
}
