using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyApp.Application.DTOs.Common;
using MyApp.Application.Interfaces.Infrastructure;
using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyApp.Infrastructure.Security
{
    public class JwtTokenService : IJwtHandler
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<JwtTokenService> _logger;
        private readonly IGenericRepository<RefreshTokensEntity> _refreshTokenRepository;

        public JwtTokenService(
            ILogger<JwtTokenService> logger,
            IOptions<JwtSettings> jwtSettings,
            IGenericRepository<RefreshTokensEntity> refreshTokenRepository)
        {
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: credentials);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                _logger.LogInformation("Token de acceso generado correctamente para el usuario con claims: {Claims}", string.Join(", ", claims.Select(c => $"{c.Type}:{c.Value}")));

                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el token de acceso.");
                throw;
            }
        }

        public async Task<RefreshTokensEntity> GenerateRefreshToken()
        {
            try
            {
                string token;
                RefreshTokensEntity? existingToken;
                do
                {
                    token = Guid.NewGuid().ToString("N");

                    existingToken = await _refreshTokenRepository.GetByCondition(x => x.Token == token);
                }
                while (existingToken != null);

                var refreshToken = new RefreshTokensEntity
                {
                    IsActive = true,
                    Token = token,
                    TokenExpirationDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationHours),
                };

                _logger.LogInformation("Token de refresco generado exitosamente.");

                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el token de refresco.");
                throw;
            }
        }
    }
}
