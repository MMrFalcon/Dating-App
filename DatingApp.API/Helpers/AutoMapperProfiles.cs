using System.Linq;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, options =>
                options.MapFrom(source => source.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest =>  dest.Age, options => options.MapFrom(source => source.DateOfBirth.CalculateAge()));
                
            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, options =>
                options.MapFrom(source => source.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest =>  dest.Age, options => options.MapFrom(source => source.DateOfBirth.CalculateAge()));

            CreateMap<Photo, PhotosForDetailedDto>();
            
            CreateMap<UserForUpdateDto, User>();

            CreateMap<Photo, PhotoForReturnDto>();

            CreateMap<PhotoForCreationDto, Photo>();

            CreateMap<UserForRegisterDto, User>();
        }
    }
}