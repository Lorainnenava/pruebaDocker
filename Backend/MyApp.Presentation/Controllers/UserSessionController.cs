using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs.UserSessions;
using MyApp.Application.Interfaces.UseCases.UserSessions;
using System.Net;

namespace MyApp.Presentation.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UserSessionController : ControllerBase
    {
        public readonly IUserSessionsCreateUseCase _userSessionsCreateUseCase;
        public readonly IUserSessionRevokedUseCase _userSessionRevokedUseCase;

        public UserSessionController(
            IUserSessionsCreateUseCase userSessionsCreateUseCase,
            IUserSessionRevokedUseCase userSessionRevokedUseCase)
        {
            _userSessionsCreateUseCase = userSessionsCreateUseCase;
            _userSessionRevokedUseCase = userSessionRevokedUseCase;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] UserSessionRequest request)
        {
            string? ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(ip))
            {
                var remoteIp = HttpContext.Connection.RemoteIpAddress;

                if (remoteIp == null)
                {
                    throw new Exception("No se pudo detectar la dirección IP del cliente.");
                }

                if (IPAddress.IsLoopback(remoteIp))
                {
                    ip = "127.0.0.1";
                }
                else
                {
                    ip = remoteIp.ToString();
                }
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                throw new Exception("No se pudo detectar la dirección IP del cliente.");
            }

            request.IpAddress = ip;

            var result = await _userSessionsCreateUseCase.Execute(request);
            return Ok(result);
        }


        [HttpGet("logout/{refreshToken}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIdUser(string refreshToken)
        {
            var result = await _userSessionRevokedUseCase.Execute(refreshToken);
            return Ok(result);
        }
    }
}
