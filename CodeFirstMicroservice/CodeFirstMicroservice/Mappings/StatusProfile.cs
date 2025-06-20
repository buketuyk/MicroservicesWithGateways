using AutoMapper;
using CodeFirstMicroservice.Models;
using CodeFirstMicroservice.Models.Dtos;

namespace CodeFirstMicroservice.Mappings
{
    public class StatusProfile : Profile
    {
        public StatusProfile()
        {
            CreateMap<Status, StatusDto>().ReverseMap();
        }
    }
}
