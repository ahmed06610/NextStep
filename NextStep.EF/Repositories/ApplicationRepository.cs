using Microsoft.EntityFrameworkCore;
using NextStep.Core.Interfaces;
using NextStep.Core.Models;
using NextStep.EF.Data;

namespace NextStep.EF.Repositories
{
    public class ApplicationRepository : BaseRepository<Application>, IApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Application> GetByIdWithStepsAsync(int id)
        {
            return await _context.Applications
                .Include(a => a.Steps)
                .FirstOrDefaultAsync(a => a.ApplicationID == id);
        }
        public IQueryable<Application> GetByCurrentDepartmentQueryable(
      int departmentId,
      string search = null,
      int? requestType = null,
      string status = null)
        {
            var query = _context.Applications
                .Include(a => a.ApplicationType)
                .Include(a => a.Steps)
                    .ThenInclude(s => s.Department)
                .Include(a => a.ApplicationHistories)
                    .ThenInclude(h => h.Department)
                .Include(a => a.CreatedByUser)
                    .ThenInclude(e => e.Department)
                .Where(a =>
                    ((a.Steps.DepartmentID == departmentId && a.Status == "قيد_التنفيذ") ||
                    (a.CreatedByUser.DepartmentID == departmentId &&
                     (a.Status == "مقبول" || a.Status == "مرفوض")))
                    && a.IsDone == false
                );

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.ApplicationID.ToString().Contains(search) ||
                                         a.ApplicationType.ApplicationTypeName.Contains(search));
            }

            if (requestType.HasValue)
            {
                query = query.Where(a => a.ApplicationTypeID == requestType.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            return query.OrderByDescending(a => a.CreatedDate);
        }


        public IQueryable<Application> GetByCreatorOrActionDepartmentQueryable(
     int departmentId,
     string search = null,
     int? requestType = null,
     string status = null)
        {
            var query = _context.Applications
                .Include(a => a.ApplicationType)
                .Include(a => a.Steps)
                    .ThenInclude(s => s.Department)
                .Include(a => a.ApplicationHistories)
                    .ThenInclude(h => h.Department)
                .Include(a => a.CreatedByUser)
                    .ThenInclude(e => e.Department)
                .Where(a =>
                    a.ApplicationType.CreatedByDeptId == departmentId ||
                    a.ApplicationHistories.Any(h =>
                        h.Department.DepartmentID == departmentId &&
                        (h.Action == "موافقة" || h.Action == "رفض")
                    )
                );

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.ApplicationID.ToString().Contains(search) ||
                                         a.ApplicationType.ApplicationTypeName.Contains(search));
            }

            if (requestType.HasValue)
            {
                query = query.Where(a => a.ApplicationTypeID == requestType.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            return query.OrderByDescending(a => a.CreatedDate);
        }


    }

}
