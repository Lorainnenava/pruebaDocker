using AutoMapper;
using MyApp.Application.DTOs.Users;
using MyApp.Domain.Entities;

namespace MyApp.Application.Mappers
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<UserCreateRequest, UsersEntity>();
            CreateMap<UsersEntity, UserResponse>();
            CreateMap<UserUpdateRequest, UsersEntity>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) =>
                    {
                        if (srcMember == null) return false;
                        if (srcMember is string s && string.IsNullOrWhiteSpace(s))
                            return false;
                        return true;
                    }));
        }
    }
}
