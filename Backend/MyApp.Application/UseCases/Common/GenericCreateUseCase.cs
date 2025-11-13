using AutoMapper;
using Microsoft.Extensions.Logging;
using MyApp.Application.Interfaces.UseCases.Common;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using MyApp.Shared.Interfaces;
using MyApp.Shared.Services;

namespace MyApp.Application.UseCases.Common
{
    public class GenericCreateUseCase<TEntity, TResponse> : IGenericCreateUseCase<TEntity, TResponse>
        where TResponse : class
        where TEntity : class, INameable, new()
    {
        private readonly IGenericRepository<TEntity> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GenericCreateUseCase<TEntity, TResponse>> _logger;

        public GenericCreateUseCase(
            IGenericRepository<TEntity> repository,
            IMapper mapper,
            ILogger<GenericCreateUseCase<TEntity, TResponse>> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TResponse> Excecute(string Name)
        {
            _logger.LogInformation("Iniciando la creación de un registro de tipo {EntityType}", typeof(TEntity).Name);

            var normalizedName = NormalizeService.Normalize(Name);

            var all = await _repository.GetAll();
            var exists = all.FirstOrDefault(x => NormalizeService.Normalize(x.Name) == normalizedName);

            if (exists is not null)
            {
                _logger.LogWarning("Ya existe un registro con este nombre {name}", Name);
                throw new AlreadyExistsException("Ya existe un registro con este nombre.");
            }

            TEntity data = new()
            {
                Name = Name,
            };

            TEntity createEntity = await _repository.Create(data);

            var response = _mapper.Map<TResponse>(createEntity);

            _logger.LogInformation("El registro fue creado exitosamente {@Response}", response);

            return response;
        }
    }
}
