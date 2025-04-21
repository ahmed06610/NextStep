using NextStep.Core.DTOs.ApplicationType;
using NextStep.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.Interfaces.Services
{
    public interface IApplicationTypeService
    {
        Task<IEnumerable<ApplicationTypeDTO>> GetAllAsync(int departmentid);
        Task<ApplicationTypeDTO> GetByIdAsync(int id);
        Task<ApplicationTypeDTO> CreateAsync(CreateApplicationTypeDTO dto);
        Task<ApplicationTypeDTO> UpdateAsync(UpdateApplicationTypeDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
