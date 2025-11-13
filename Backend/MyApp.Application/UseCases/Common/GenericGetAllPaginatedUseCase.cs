using AutoMapper;
using Microsoft.Extensions.Logging;
using MyApp.Application.Interfaces.UseCases.Common;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.DTOs;

namespace MyApp.Application.UseCases.Common
{
    public class GenericGetAllPaginatedUseCase<TEntity, TResponse> : IGenericGetAllPaginatedUseCase<TEntity, TResponse>
        where TEntity : class
        where TResponse : class
    {
        private readonly IGenericRepository<TEntity> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GenericGetAllPaginatedUseCase<TEntity, TResponse>> _logger;

        public GenericGetAllPaginatedUseCase(
            IGenericRepository<TEntity> repository,
            IMapper mapper,
            ILogger<GenericGetAllPaginatedUseCase<TEntity, TResponse>> logger)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaginationResult<TResponse>> Execute(int page, int size)
        {
            _logger.LogInformation("Iniciando proceso de paginación para registros de tipo {EntityName}.", typeof(TEntity).Name);

            var (items, totalCount) = await _repository.Pagination(page, size);

            var mappedItems = _mapper.Map<IEnumerable<TResponse>>(items);

            _logger.LogInformation("Se obtuvieron {Count} registros de tipo {EntityName}.", totalCount, typeof(TEntity).Name);

            return new PaginationResult<TResponse>
            {
                RowsCount = totalCount,
                PageCount = (int)Math.Ceiling((double)totalCount / size),
                PageSize = size,
                CurrentPage = page,
                Results = mappedItems
            };
        }
    }
}
