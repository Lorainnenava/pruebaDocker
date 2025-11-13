using Microsoft.Extensions.Logging;
using MyApp.Application.Interfaces.UseCases.UserSessions;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;

namespace MyApp.Application.UseCases.UserSessions
{
    public class UserSessionRevokedUseCase : IUserSessionRevokedUseCase
    {
        private readonly IGenericRepository<RefreshTokensEntity> _refreshTokensRepository;
        private readonly IGenericRepository<UserSessionsEntity> _userSessionsRepository;
        private readonly ILogger<UserSessionRevokedUseCase> _logger;
        public UserSessionRevokedUseCase(
            IGenericRepository<RefreshTokensEntity> refreshTokensRepository,
            IGenericRepository<UserSessionsEntity> userSessionsRepository,
            ILogger<UserSessionRevokedUseCase> logger)
        {
            _logger = logger;
            _userSessionsRepository = userSessionsRepository;
            _refreshTokensRepository = refreshTokensRepository;
        }

        public async Task<bool> Execute(string RefreshToken)
        {
            _logger.LogInformation("Iniciando proceso para revokar la sesión del usuario con refresh token: {RefreshToken}", RefreshToken);

            var searchRefreshToken = await _refreshTokensRepository.GetByCondition(x => x.Token == RefreshToken, x => x.UserSession);

            if (searchRefreshToken is null)
            {
                _logger.LogWarning("No se encontró ningún refresh token con el valor: {RefreshToken}", RefreshToken);
                throw new NotFoundException("No existe ningun refresh token con este valor.");
            }

            if (searchRefreshToken.UserSession is null)
            {
                _logger.LogWarning("No se encontró ninguna sesión con el UserSessionId: {UserSessionId}", searchRefreshToken.UserSessionId);
                throw new NotFoundException("No existe ninguna sessión asociada a este refresh token.");
            }

            searchRefreshToken.IsActive = false;
            searchRefreshToken.UpdatedAt = DateTime.UtcNow;

            searchRefreshToken.UserSession.IsRevoked = true;
            searchRefreshToken.UserSession.UpdatedAt = DateTime.UtcNow;

            await _refreshTokensRepository.Update(searchRefreshToken);
            await _userSessionsRepository.Update(searchRefreshToken.UserSession);

            _logger.LogInformation("Sesión eliminada exitosamente.");

            return true;
        }
    }
}