using AllStars.API.DTO.User;
using AutoMapper;

namespace AllStars.API.Profiles;

public class UserNickNamesResponseProfile : Profile
{
    public UserNickNamesResponseProfile()
    {
        CreateMap<IEnumerable<string>, UserNickNamesResponse>()
            .ForMember(dest => dest.NickNames, opt => opt.MapFrom(src => src));
    }
}