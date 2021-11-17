using AutoMapper;
using PlatformService.DTOs;
using PlatformService.Model;

namespace PlatformService.Profiles
{
    public class PlatformProfile: Profile
    {
        public PlatformProfile()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformCreateDto, Platform>();
            CreateMap<PlatformReadDto, PlatformPublishedDto>();
            CreateMap<Platform, GrpcPlatformModel>().
                ForMember(dest => dest.PlatformId, opt => opt.MapFrom(src => src.Id));
        }
    }
}
