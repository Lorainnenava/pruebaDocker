using MyApp.Application.DTOs.Users;
using AutoMapper;
using MyApp.Domain.Entities;

namespace MyApp.Application.Mappers
{
    public class UserSessionsProfile : Profile
    {
        public UserSessionsProfile()
        {
            CreateMap<UserCreateRequest, UserSessionsEntity>();
            CreateMap<UserSessionsEntity, UserResponse>();
        }
    }
}
