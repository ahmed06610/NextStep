using NextStep.Core.DTOs.Application;
using NextStep.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.Interfaces.Services
{
    public interface IApplicationService
    {
        Task<Application> CreateApplicationAsync(CreateApplicationDTO dto, int employeeId);
        Task<bool> ApproveApplicationAsync(ApplicationActionDTO dto, int employeeId, int departmentId);
        Task<bool> RejectApplicationAsync(ApplicationActionDTO dto, int employeeId, int departmentId);
        Task<ApplicationDetailsDTO> GetApplicationDetailsAsync(int applicationId);
        Task<List<ApplicationStudent>> GetApplicationsForStuent(int StudentId);


        Task<InboxResponseDTO> GetInboxApplicationsAsync(
           int departmentId,
           bool isOrderCreatingDepartment,
           string search = null,
           int? requestType = null,
           string status = null,
           int page = 1,
           int limit = 10);
        Task<OutboxResponseDTO> GetOutboxApplicationsAsync(
                int departmentId,
                string search = null,
                int? requestType = null,
                string status = null,
                int page = 1,
                int limit = 10);
    }
}
