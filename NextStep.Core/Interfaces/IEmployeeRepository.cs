using NextStep.Core.Models;

namespace NextStep.Core.Interfaces
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<Employee> GetByAppUserIdAsync(string id);

    }

}
