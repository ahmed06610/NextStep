using NextStep.Core.DTOs.Department;

namespace NextStep.Core.Interfaces.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDTO>> GetAllAsync();
        Task<DepartmentDTO> CreateAsync(CreateDepartmentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
