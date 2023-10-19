
using AutoMapper;
using System.Linq;
using JhipsterDotNetMS.Domain.Entities;
using JhipsterDotNetMS.Dto;


namespace JhipsterDotNetMS.Configuration.AutoMapper
{

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            CreateMap<Conference, ConferenceDto>().ReverseMap();
        }
    }
}
