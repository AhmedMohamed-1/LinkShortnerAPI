using AutoMapper;
using LinkShorterAPI.DTOs.LinkDTO;
using LinkShorterAPI.Models;

namespace LinkShorterAPI.MapperProfiles.LinkMappingProfiles
{
    public class LinkMappingProfile : Profile
    {
        public LinkMappingProfile() 
        {
            CreateMap<ShortLink, ShortLinkDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DestinationUrl, opt => opt.MapFrom(src => src.DestinationUrl))
                .ForMember(dest => dest.ShortCode, opt => opt.MapFrom(src => src.Slug))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpireAt))
                .ForMember(dest => dest.ClickCount, opt => opt.MapFrom(src => src.ClickCount))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.Domain, opt => opt.MapFrom(src => src.Domain.DomainName)).ReverseMap();

            CreateMap<UpdateShortLinkDTO , ShortLink>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DestinationUrl, opt => opt.MapFrom(src => src.DestinationUrl))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.ExpireAt, opt => opt.MapFrom(src => src.ExpirationDate))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ReverseMap();

            CreateMap<CreateShortLinkDTO , ShortLink>()
                .ForMember(dest => dest.DestinationUrl, opt => opt.MapFrom(src => src.DestinationUrl))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.ExpireAt, opt => opt.MapFrom(src => src.ExpirationDate))
                .ForMember(dest => dest.DomainId, opt => opt.MapFrom(src => src.DomainId))
                .ForMember(dest => dest.Slug, opt => opt.Ignore()) // We'll set this manually in the service
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // We'll set this manually in the service
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // We'll set this manually in the service
                .ForMember(dest => dest.TeamId, opt => opt.Ignore()) // We'll set this manually in the service
                .ForMember(dest => dest.IsActive, opt => opt.Ignore()) // We'll set this manually in the service
                .ReverseMap();
        }
    }
}
