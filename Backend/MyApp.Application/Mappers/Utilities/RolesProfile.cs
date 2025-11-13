using AutoMapper;
using MyApp.Application.DTOs.Roles;
using MyApp.Domain.Entities;

namespace MyApp.Application.Mappers.Utilities
{
    public class RolesProfile : Profile
    {
        public RolesProfile()
        {
            CreateMap<RolesEntity, RoleResponse>();
        }
    }
}
