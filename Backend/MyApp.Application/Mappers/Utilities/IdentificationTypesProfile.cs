using AutoMapper;
using MyApp.Application.DTOs.Utilities.IdentificationTypes;
using MyApp.Domain.Entities;

namespace MyApp.Application.Mappers.Utilities
{
    public class IdentificationTypeProfile : Profile
    {
        public IdentificationTypeProfile()
        {
            CreateMap<IdentificationTypeRequest, IdentificationTypesEntity>();
            CreateMap<IdentificationTypesEntity, IdentificationTypeResponse>();
        }
    }
}
