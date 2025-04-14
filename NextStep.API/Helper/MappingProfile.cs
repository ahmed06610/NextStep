using AutoMapper;
using NextStep.Core.DTOs.Application;
using NextStep.Core.DTOs.ApplicationType;
using NextStep.Core.DTOs.Auth;
using NextStep.Core.Models;

namespace NextStep.API.Helper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationType, ApplicationTypeDTO>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApplicationTypeID))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ApplicationTypeName));

            CreateMap<CreateApplicationTypeDTO, ApplicationType>();
            CreateMap<UpdateApplicationTypeDTO, ApplicationType>();

            CreateMap<Application, ApplicationListItemDTO>()
    .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(src => src.ApplicationID))
    .ForMember(dest => dest.ApplicationType, opt => opt.MapFrom(src => src.ApplicationType.ApplicationTypeName))
    .ForMember(dest => dest.SendingDepartment, opt => opt.MapFrom(src => src.Steps.Department.DepartmentName))
    .ForMember(dest => dest.SentDate, opt => opt.MapFrom(src => src.CreatedDate));

            CreateMap<ApplicationHistory, HistoryItemDTO>()
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.DepartmentName));
            // CreateMap<RegisterUserDTO, ApplicationUser>();
            // CreateMap<RegisterStudentDTO, Student>();
            // CreateMap<RegisterEmployeeDTO, Employee>();
            // CreateMap<RegisterStudentDTO, RegisterUserDTO>();
            // CreateMap<RegisterEmployeeDTO, RegisterUserDTO>();
            // CreateMap<Application, ApplicationDTO>();
            // CreateMap<ApplicationDTO, Application>();
            // CreateMap<Department, DepartmentDTO>();
            // CreateMap<DepartmentDTO, Department>();
        }
    }
}
