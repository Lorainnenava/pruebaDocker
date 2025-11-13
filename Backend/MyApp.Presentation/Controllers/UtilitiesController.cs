using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs.Roles;
using MyApp.Application.DTOs.Utilities.Genders;
using MyApp.Application.DTOs.Utilities.IdentificationTypes;
using MyApp.Application.Interfaces.UseCases.Common;
using MyApp.Domain.Entities;

namespace MyApp.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilitiesController : ControllerBase
    {
        public readonly IGenericGetAllUseCase<RolesEntity, RoleResponse> _roleGetAll;
        public readonly IGenericGetAllUseCase<GendersEntity, GenderResponse> _genderGetAll;
        public readonly IGenericGetAllUseCase<IdentificationTypesEntity, IdentificationTypeResponse> _identityTypeGetAll;

        public UtilitiesController(
            IGenericGetAllUseCase<RolesEntity, RoleResponse> roleGetAll,
            IGenericGetAllUseCase<GendersEntity, GenderResponse> genderGetAll,
            IGenericGetAllUseCase<IdentificationTypesEntity, IdentificationTypeResponse> identificationType)
        {
            _roleGetAll = roleGetAll;
            _genderGetAll = genderGetAll;
            _identityTypeGetAll = identificationType;
        }

        [HttpGet("roles/getAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleGetAll.Execute();
            return Ok(result);
        }

        [HttpGet("identityTypes/getAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllIdentityTypes()
        {
            var result = await _identityTypeGetAll.Execute();
            return Ok(result);
        }

        [HttpGet("genders/getAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllGenders()
        {
            var result = await _genderGetAll.Execute();
            return Ok(result);
        }
    }
}
