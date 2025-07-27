using AutoMapper;
using LinkShorterAPI.DTOs.UserDTO;
using LinkShorterAPI.Models;

namespace LinkShorterAPI.MapperProfiles.UserMappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<SignUpDTO, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

            CreateMap<UpdateUserDTO, User>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.NewPassword));

            CreateMap<User, GetUserDTO>().ReverseMap();

            CreateMap<User, SignInSignUpResponse>();
        }
    }
}
