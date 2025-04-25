using NextStep.Core.Models;

namespace NextStep.Core.Interfaces
{
    public interface IApplicationRepository : IBaseRepository<Application>
    {
        Task<Application> GetByIdWithStepsAsync(int id);
        Task<List<Application>> GetByCurrentDepartmentAsync(
            int departmentId,
            string search = null,
            int? requestType = null,
            string status = null,
            int page = 1,
            int limit = 10);
        Task<List<Application>> GetByCreatorOrActionDepartmentAsync(
                   int departmentId,
                   string search = null,
                   int? requestType = null,
                   string status = null,
                   int page = 1,
                   int limit = 10);

    }

}
