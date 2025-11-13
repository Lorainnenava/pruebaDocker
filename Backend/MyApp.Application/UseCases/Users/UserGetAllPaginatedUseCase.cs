using Microsoft.Extensions.Logging;
using MyApp.Application.DTOs.Users;
using MyApp.Application.Interfaces.UseCases.Users;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.DTOs;

namespace MyApp.Application.UseCases.Users
{
    public class UserGetAllPaginatedUseCase : IUserGetAllPaginatedUseCase
    {
        private readonly IGenericRepository<UsersEntity> _userRepository;
        private readonly ILogger<UserGetAllPaginatedUseCase> _logger;

        public UserGetAllPaginatedUseCase(
            IGenericRepository<UsersEntity> userRepository,
            ILogger<UserGetAllPaginatedUseCase> logger)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<PaginationResult<UserListResponse>> Execute(int Page, int Size)
        {
            _logger.LogInformation("Iniciando la obtención de todos los usuarios.");


            var (items, totalCount) = await _userRepository.Pagination(
                Page, Size,
               null,
                x => x.IdentificationType,
                x => x.Role,
                x => x.Gender);

            var mappedItems = items.Select(x => new UserListResponse
            {
                UserId = x.UserId,
                FullName = string.Join(" ", new[]
                {
                    x.FirstName,
                    x.MiddleName,
                    x.LastName,
                    x.SecondName
                }.Where(s => !string.IsNullOrWhiteSpace(s))),
                IdentificationNumber = x.IdentificationNumber,
                IdentificationTypeName = x.IdentificationType.Name,
                GenderName = x.Gender.Name,
                RoleName = x.Role.Name,
                IsValidated = x.IsVerified,
                IsActived = x.IsActive,
            });

            _logger.LogInformation("Se obtuvieron {Count} usuarios.", totalCount);

            return new PaginationResult<UserListResponse>
            {
                RowsCount = totalCount,
                PageCount = (int)Math.Ceiling((double)totalCount / Size),
                PageSize = Size,
                CurrentPage = Page,
                Results = mappedItems
            };
        }
    }
}
