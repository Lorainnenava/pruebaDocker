using Microsoft.Extensions.Logging;
using MyApp.Application.DTOs.UserSessions;
using MyApp.Application.Interfaces.Infrastructure;
using MyApp.Application.Interfaces.UseCases.RefreshTokens;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using MyApp.Shared.Exceptions;
using System.Security.Claims;

namespace MyApp.Application.UseCases.RefreshTokens
{
    public class RefreshTokenUseCase : IRefreshTokenUseCase
    {
        private readonly IJwtHandler _jwtHandler;
        private readonly ILogger<RefreshTokenUseCase> _logger;
        private readonly IGenericRepository<UserSessionsEntity> _userSessionsRepository;
        private readonly IGenericRepository<RefreshTokensEntity> _refreshTokensRepository;

        public RefreshTokenUseCase(
            IGenericRepository<RefreshTokensEntity> refreshTokensRepository,
            IGenericRepository<UserSessionsEntity> userSessionsRepository,
            IJwtHandler jwtHandler,
            ILogger<RefreshTokenUseCase> logger)
        {
            _logger = logger;
            _jwtHandler = jwtHandler;
            _userSessionsRepository = userSessionsRepository;
            _refreshTokensRepository = refreshTokensRepository;
        }

        public async Task<UserSessionResponse> Execute(string RefreshToken)
        {
            _logger.LogInformation("Iniciando la actualización de token para refresh token: {RefreshToken}", RefreshToken);

            var searchRefreshToken = await _refreshTokensRepository.GetByCondition(
                x => x.Token == RefreshToken,
                x => x.UserSession!,
                x => x.UserSession!.User!,
                x => x.UserSession!.User!.Role);

            if (searchRefreshToken is null || searchRefreshToken.IsActive == false)
            {
                _logger.LogWarning("Refresh token no encontrado: {RefreshToken}", RefreshToken);
                throw new NotFoundException("No se pudo validar tu sesión. Por favor, vuelve a iniciar sesión.");
            }

            if (searchRefreshToken!.TokenExpirationDate <= DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token inválido o expirado. Token: {RefreshToken}", RefreshToken);

                if (searchRefreshToken?.UserSessionId is not null)
                {
                    searchRefreshToken.IsActive = false;
                    searchRefreshToken.UpdatedAt = DateTime.UtcNow;

                    searchRefreshToken.UserSession.IsRevoked = true;
                    searchRefreshToken.UserSession.UpdatedAt = DateTime.UtcNow;

                    await _userSessionsRepository.Update(searchRefreshToken.UserSession);
                    await _refreshTokensRepository.Update(searchRefreshToken);

                    _logger.LogInformation("Sesión y token eliminados para UserSessionId: {UserSessionId}", searchRefreshToken.UserSessionId);
                }

                throw new UnauthorizedAccessException("Tu sesión ha expirado. Por favor, inicia sesión nuevamente.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Sid, searchRefreshToken.UserSession.UserId.ToString()),
                new(ClaimTypes.Role, searchRefreshToken.UserSession.User.Role.Name),
            };

            var generateAccessToken = _jwtHandler.GenerateAccessToken(claims);

            _logger.LogInformation("Nuevo token generado exitosamente para el usuario con el UserId: {UserId}", searchRefreshToken.UserSession.UserId);

            return new UserSessionResponse { AccessToken = generateAccessToken, RefreshToken = RefreshToken };
        }
    }
}
