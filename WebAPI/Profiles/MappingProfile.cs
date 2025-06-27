using AutoMapper;
using OnlineConsultationApp.core.DTOs;
using WebAPI.Models;

namespace WebAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {// Area mappings
            CreateMap<Area, AreaDTO>().ReverseMap();
            CreateMap<AreaCreateDTO, Area>();

            // TypeOfWork mappings
            CreateMap<TypeOfWork, TypeOfWorkDTO>().ReverseMap();
            CreateMap<TypeOfWorkCreateDTO, TypeOfWork>();

            // Mentor mappings
            CreateMap<Mentor, MentorDTO>()
                .ForMember(dest => dest.TypeOfWorkName, opt => opt.MapFrom(src => src.TypeOfWork.Name))
                .ForMember(dest => dest.AreaNames, opt => opt.MapFrom(src => src.Areas.Select(a => a.Name)));

            CreateMap<MentorDTO, Mentor>();

            CreateMap<MentorCreateDTO, Mentor>()
                .ForMember(dest => dest.Areas, opt => opt.Ignore());

            CreateMap<User, AuthenticatedUserDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname));

            CreateMap<AuthenticatedUserDTO, User>();

            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserCreateDTO>().ReverseMap();

            // If you want to map RegisterUserDTO to User for registration logic
            CreateMap<RegisterUserDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Hash password manually in service/controller
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "User")); // Assign default role

            // Consultation mappings
            CreateMap<Consultation, ConsultationDTO>()
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.Name + " " + src.Mentor.Surname))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name + " " + src.User.Surname));

            CreateMap<ConsultationDTO, Consultation>();

            CreateMap<ConsultationCreateDTO, Consultation>().ReverseMap();
            CreateMap<ConsultationInfoDTO, Consultation>().ReverseMap();

            CreateMap<UserWithConsultationsDTO, User>().ReverseMap();
        }
    }
}
