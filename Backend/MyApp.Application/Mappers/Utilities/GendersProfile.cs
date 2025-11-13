using AutoMapper;
using MyApp.Application.DTOs.Utilities.Genders;
using MyApp.Domain.Entities;

namespace MyApp.Application.Mappers.Utilities
{
    public class GenderProfile : Profile
    {
        public GenderProfile()
        {
            CreateMap<GenderRequest, GendersEntity>();
            CreateMap<GendersEntity, GenderResponse>();
        }
    }
}
