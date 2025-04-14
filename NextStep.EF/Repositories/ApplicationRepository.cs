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
        public async Task<List<Application>> GetByCurrentDepartmentAsync(int departmentId)
        {
            return await _context.Applications
                .Include(a => a.ApplicationType)
                .Include(a => a.Steps)
                    .ThenInclude(s => s.Department)
                .Include(a => a.ApplicationHistories)
                    .ThenInclude(h => h.Department)
                .Include(a => a.CreatedByUser)
                    .ThenInclude(e => e.Department)
                .Where(a =>
                    // Applications where this department is the current step
                    (a.Steps.DepartmentID == departmentId &&
                     a.Status == "قيد_التنفيذ" ||

                    // OR applications that were finalized (approved/rejected) 
                    // AND were originally created by this department
                    (a.CreatedByUser.DepartmentID == departmentId &&
                     (a.Status == "مقبول" ||
                      a.Status == "مرفوض"))
                ))
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Application>> GetByCreatorOrActionDepartmentAsync(int departmentId)
        {
            return await _context.Applications
                .Include(a => a.ApplicationType)
                .Include(a => a.Steps)
                    .ThenInclude(s => s.Department)
                .Include(a => a.ApplicationHistories)
                    .ThenInclude(h => h.Department)
                .Include(a => a.CreatedByUser)
                    .ThenInclude(e => e.Department)
                .Where(a =>
                    // Applications created by this department
                    a.CreatedByUser.DepartmentID == departmentId ||
                    // OR applications where this department took action
                    a.ApplicationHistories.Any(h =>
                        h.Department.DepartmentID == departmentId &&
                        (h.Action == "موافقة" || h.Action == "رفض")
                    )
                )
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }
    }

}
