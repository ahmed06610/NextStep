using AutoMapper;
using NextStep.Core.DTOs.Application;
using NextStep.Core.DTOs.ApplicationType;
using NextStep.Core.DTOs.Auth;
using NextStep.Core.DTOs.Department;
using NextStep.Core.DTOs.Employee;
using NextStep.Core.Models;

namespace NextStep.API.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping for ApplicationType to ApplicationTypeDTO
            CreateMap<ApplicationType, ApplicationTypeDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApplicationTypeID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ApplicationTypeName))
                .ForMember(dest => dest.Requierments, opt => opt.MapFrom(src => src.Requierments.Select(r => new RequiermentDTO
                {
                    Id = r.Requierment.Id,
                    Name = r.Requierment.RequiermentName
                })))
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps.Select(s => new StepDTO
                {
                    Id = s.StepsID,
                    DepartmentId = s.DepartmentID,
                    StepOrder = s.StepOrder
                })))
                .ReverseMap();

            // Mapping for CreateApplicationTypeDTO to ApplicationType
            CreateMap<CreateApplicationTypeDTO, ApplicationType>()
                .ForMember(dest => dest.Steps, opt => opt.Ignore()) // Steps are handled separately
                .ForMember(dest => dest.Requierments, opt => opt.Ignore()); // Requirements are handled separately

            // Mapping for UpdateApplicationTypeDTO to ApplicationType
            CreateMap<UpdateApplicationTypeDTO, ApplicationType>()
                .ForMember(dest => dest.Steps, opt => opt.Ignore())
                .ForMember(dest => dest.Requierments, opt => opt.Ignore());

            // Mapping for CreateRequiermentDTO to Requierments
            CreateMap<CreateRequiermentDTO, Requierments>();

            // Mapping for CreateStepsDTO to Steps
            CreateMap<CreateStepsDTO, Steps>()
                .ForMember(dest => dest.StepsID, opt => opt.Ignore()) // StepsID is auto-generated
                .ForMember(dest => dest.ApplicationTypeID, opt => opt.Ignore()); // ApplicationTypeID is set separately
        

        CreateMap<Application, ApplicationListItemDTO>()
                .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(src => src.ApplicationID))
                .ForMember(dest => dest.ApplicationType, opt => opt.MapFrom(src => src.ApplicationType.ApplicationTypeName))
                .ForMember(dest => dest.SendingDepartment, opt => opt.MapFrom(src => src.Steps.Department.DepartmentName))
                .ForMember(dest => dest.SentDate, opt => opt.MapFrom(src => src.CreatedDate));

            CreateMap<ApplicationHistory, HistoryItemDTO>()
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department.DepartmentName));

            // New mappings
            CreateMap<Department, DepartmentDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DepartmentID))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.DepartmentName));

            CreateMap<CreateDepartmentDTO, Department>();

            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EmpID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));

            CreateMap<UpdateEmployeeDTO, Employee>()
                .ForMember(dest => dest.User, opt => opt.Ignore()); // User updates are handled separately
        }
    }
}
