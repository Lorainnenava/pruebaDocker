using MyApp.Domain.Entities;
using System.Security.Claims;

namespace MyApp.Application.Interfaces.Infrastructure
{
    public interface IJwtHandler
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        Task<RefreshTokensEntity> GenerateRefreshToken();
    }
}