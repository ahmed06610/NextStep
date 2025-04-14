using NextStep.Core.Models;

namespace NextStep.Core.Interfaces
{
    public interface IApplicationRepository : IBaseRepository<Application>
    {
        Task<Application> GetByIdWithStepsAsync(int id);
        Task<List<Application>> GetByCurrentDepartmentAsync(int departmentId);
        Task<List<Application>> GetByCreatorOrActionDepartmentAsync(int departmentId);


    }

}
