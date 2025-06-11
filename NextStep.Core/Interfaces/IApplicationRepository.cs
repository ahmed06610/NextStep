using NextStep.Core.Models;

namespace NextStep.Core.Interfaces
{
    public interface IApplicationRepository : IBaseRepository<Application>
    {
        Task<Application> GetByIdWithStepsAsync(int id);
        IQueryable<Application> GetByCurrentDepartmentQueryable(
            int departmentId,
            string search = null,
            int? requestType = null,
            string status = null);
        IQueryable<Application> GetByCreatorOrActionDepartmentQueryable(
                   int departmentId,
                   string search = null,
                   int? requestType = null,
                   string status = null);

    }

}
