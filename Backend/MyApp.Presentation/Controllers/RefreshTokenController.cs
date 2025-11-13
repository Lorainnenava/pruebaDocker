using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Interfaces.UseCases.RefreshTokens;

namespace MyApp.Presentation.Controllers
{
    [Route("")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        public readonly IRefreshTokenUseCase _refreshTokenService;

        public RefreshTokenController(IRefreshTokenUseCase refreshTokenService)
        {
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] string RefreshToken)
        {
            var result = await _refreshTokenService.Execute(RefreshToken);
            return Ok(result);
        }
    }
}
