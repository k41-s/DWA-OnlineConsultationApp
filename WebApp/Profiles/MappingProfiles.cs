using AutoMapper;
using OnlineConsultationApp.core.DTOs;
using WebApp.ViewModels;

namespace WebApp.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Area
            CreateMap<AreaViewModel, AreaDTO>().ReverseMap();

            // Consultation
            CreateMap<ConsultationViewModel, ConsultationDTO>().ReverseMap();

            // Login (usually one-way, no reverse mapping needed)
            CreateMap<LoginViewModel, LoginDTO>();
            CreateMap<UserDTO, ProfileViewModel>().ReverseMap();

            // Mentor
            CreateMap<MentorViewModel, MentorDTO>().ReverseMap();
            CreateMap<MentorViewModel, MentorCreateDTO>().ReverseMap();

            // MyConsultation
            CreateMap<MyConsultationViewModel, ConsultationDTO>().ReverseMap();

            // Register (usually one-way, for sending data to API)
            CreateMap<RegisterViewModel, RegisterUserDTO>();

            // TypeOfWork
            CreateMap<TypeOfWorkViewModel, TypeOfWorkDTO>().ReverseMap();

            CreateMap<ConsultationDTO, ConsultationInfoViewModel>();

            CreateMap<UserWithConsultationsDTO, UserWithConsultationsViewModel>().ReverseMap();
            CreateMap<ConsultationInfoDTO, ConsultationInfoViewModel>().ReverseMap();

            CreateMap<AreaCreateDTO, AreaViewModel>().ReverseMap();
        }
    }
}
