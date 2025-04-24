using NextStep.Core.DTOs.Employee;

namespace NextStep.Core.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetAllAsync();
        Task<bool> UpdateAsync(int id, UpdateEmployeeDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
