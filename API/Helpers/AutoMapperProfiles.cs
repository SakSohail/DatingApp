using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;
using System.Linq;

namespace API.Helpers
{
    //instsall nuget - AutoMapper.Extensions.Microsoft.DependencyInjection
    //auto mapper maps one object another
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // CreateMap<AppUser, MemberDto>();//map from AppUser to MemberDto 

            //when we want to map individual property ,then we pass dest property photourl,then we tell where to mapFrom which is we add source from where we are mapping from ,we goes to user phot collections,get first photo which is isMain and get url
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
                    src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));//we are calculating age here only
                

            CreateMap<Photo, PhotoDto>();//map from photo entity to photoDto
            //after this inject into services 
        }
    }
}
