using Microsoft.Extensions.Logging;
using MyApp.Application.Interfaces.UseCases.Users;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;

namespace MyApp.Application.UseCases.Users
{
    public class UserSetActiveStatusUseCase : IUserSetActiveStatusUseCase
    {
        private readonly IGenericRepository<UsersEntity> _userRepository;
        private readonly ILogger<UserSetActiveStatusUseCase> _logger;

        public UserSetActiveStatusUseCase(
            IGenericRepository<UsersEntity> userRepository,
            ILogger<UserSetActiveStatusUseCase> logger
            )
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<bool> Execute(int UserId)
        {
            _logger.LogInformation("Intentando cambiar el estado activo/inactivo del usuario con ID: {UserId}", UserId);

            UsersEntity? searchUser = await _userRepository.GetByCondition(u => u.UserId == UserId);

            if (searchUser is null)
            {
                _logger.LogWarning("No se encontró ningún usuario con ID: {UserId}", UserId);
                throw new NotFoundException("Este usuario no existe o ha sido eliminado.");
            }

            if(!searchUser.IsVerified)
            {
                _logger.LogWarning("No se puede cambiar el estado de este usuario ya que no se ha validado");
                throw new InvalidOperationException("No se puede cambiar el estado del usuario porque aún no ha validado su cuenta.");
            }

            searchUser.IsActive = !searchUser.IsActive;
            searchUser.UpdatedAt = DateTime.UtcNow;

            var updateEntity = await _userRepository.Update(searchUser);

            _logger.LogInformation("Usuario con ID: {UserId} ahora está {Estado}", UserId, updateEntity.IsActive ? "activo" : "inactivo");

            return true;
        }
    }
}
