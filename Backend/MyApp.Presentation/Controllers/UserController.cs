using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs.UserPasswordResets;
using MyApp.Application.DTOs.Users;
using MyApp.Application.DTOs.UserVerifications;
using MyApp.Application.Interfaces.UseCases.UserPasswordResets;
using MyApp.Application.Interfaces.UseCases.Users;
using MyApp.Application.Interfaces.UseCases.UserVerifications;
using MyApp.Shared.DTOs;

namespace MyApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserUpdateUseCase _userUpdateUseCase;
        private readonly IUserCreateUseCase _userCreateUseCase;
        private readonly IUserGetByIdUseCase _userGetByIdUseCase;
        private readonly IUserValidateUseCase _userValidateUseCase;
        private readonly IUserGetAllPaginatedUseCase _userGetAllUseCase;
        private readonly IUserChangePasswordUseCase _userChangePasswordUseCase;
        private readonly IUserSetActiveStatusUseCase _userSetActiveStatusUseCase;
        private readonly IResendVerificationCodeUseCase _resendVerificationCodeUseCase;
        private readonly IUserPasswordResetRequestUseCase _userPasswordResetRequestUseCase;
        private readonly IUserPasswordResetConfirmUseCase _userPasswordResetConfirmUseCase;
        private readonly IUserPasswordResetValidateCodeUseCase _userPasswordResetValidateCodeUseCase;

        public UserController(
            IUserUpdateUseCase userUpdateUseCase,
            IUserCreateUseCase userCreateUseCase,
            IUserGetByIdUseCase userGetByIdUseCase,
            IUserGetAllPaginatedUseCase userGetAllUseCase,
            IUserChangePasswordUseCase userChangePasswordUseCase,
            IUserSetActiveStatusUseCase userSetActiveStatusUseCase,
            IResendVerificationCodeUseCase resendVerificationCodeUseCase,
            IUserPasswordResetRequestUseCase userPasswordResetRequestUseCase,
            IUserPasswordResetConfirmUseCase userPasswordResetConfirmUseCase,
            IUserPasswordResetValidateCodeUseCase userPasswordResetValidateCodeUseCase,
            IUserValidateUseCase userValidateUseCase)
        {
            _userCreateUseCase = userCreateUseCase;
            _userGetAllUseCase = userGetAllUseCase;
            _userUpdateUseCase = userUpdateUseCase;
            _userGetByIdUseCase = userGetByIdUseCase;
            _userValidateUseCase = userValidateUseCase;
            _userChangePasswordUseCase = userChangePasswordUseCase;
            _userSetActiveStatusUseCase = userSetActiveStatusUseCase;
            _resendVerificationCodeUseCase = resendVerificationCodeUseCase;
            _userPasswordResetConfirmUseCase = userPasswordResetConfirmUseCase;
            _userPasswordResetRequestUseCase = userPasswordResetRequestUseCase;
            _userPasswordResetValidateCodeUseCase = userPasswordResetValidateCodeUseCase;
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest request)
        {
            var result = await _userCreateUseCase.Execute(request);
            return CreatedAtAction(nameof(GetByIdUser), new { UserId = result.UserId }, result);
        }

        [HttpGet("getById/{UserId}")]
        [Authorize]
        public async Task<IActionResult> GetByIdUser(int UserId)
        {
            var result = await _userGetByIdUseCase.Execute(UserId);
            return Ok(result);
        }

        [HttpGet("getAllPaginated")]
        public async Task<ActionResult<PaginationResult<UserResponse>>> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            var result = await _userGetAllUseCase.Execute(page, size);
            return Ok(result);
        }

        [HttpPut("update/{UserId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int UserId, [FromBody] UserUpdateRequest request)
        {
            var result = await _userUpdateUseCase.Execute(UserId, request);
            return Ok(result);
        }

        [HttpPut("changePassword/{UserId}")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(int UserId, [FromBody] UserChangePasswordRequest request)
        {
            var result = await _userChangePasswordUseCase.Execute(UserId, request);
            return Ok(result);
        }

        [HttpPut("setActiveStatus/{UserId}")]
        [Authorize]
        public async Task<IActionResult> SetActiveStatus(int UserId)
        {
            var result = await _userSetActiveStatusUseCase.Execute(UserId);
            return Ok(result);
        }

        [HttpPut("validate")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateUser([FromBody] UserCodeValidationRequest request)
        {
            var result = await _userValidateUseCase.Execute(request);
            return Ok(result);
        }

        [HttpPut("resendCodeValidate")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendCodeValidation([FromBody] ResendCodeRequest request)
        {
            var result = await _resendVerificationCodeUseCase.Execute(request);
            return Ok(result);
        }

        [HttpPost("forgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] UserPasswordResetRequest request)
        {
            var result = await _userPasswordResetRequestUseCase.Execute(request);
            return Ok(result);
        }

        [HttpPost("validateCodeResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateResetPassword([FromBody] UserPasswordResetValidateResetCodeRequest request)
        {
            var result = await _userPasswordResetValidateCodeUseCase.Execute(request);
            return Ok(result);
        }

        [HttpPost("resendCodeResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendCodeResetPassword([FromBody] UserPasswordResetRequest request)
        {
            var result = await _userPasswordResetRequestUseCase.Execute(request);
            return Ok(result);
        }

        [HttpPost("confirmResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmResetPassword([FromBody] UserPasswordResetConfirmRequest request)
        {
            var result = await _userPasswordResetConfirmUseCase.Execute(request);
            return Ok(result);
        }
    }
}
