using Microsoft.EntityFrameworkCore;
using NextStep.Core.Interfaces;
using NextStep.Core.Models;
using NextStep.EF.Data;

namespace NextStep.EF.Repositories
{
    public class StepsRepository : BaseRepository<Steps>, IStepsRepository
    {
        private readonly ApplicationDbContext _context;

        public StepsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Steps> GetInitialStepByApplicationType(int applicationTypeId)
        {
            return await _context.Steps
                .Where(s => s.ApplicationTypeID == applicationTypeId)
                .OrderBy(s => s.StepOrder)
                .FirstOrDefaultAsync();
        }
        public async Task<Steps> GetNextStepAsync(int applicationTypeId, int currentStepOrder)
        {
            return await _context.Steps
                .Where(s => s.ApplicationTypeID == applicationTypeId && s.StepOrder > currentStepOrder)
                .OrderBy(s => s.StepOrder)
                .FirstOrDefaultAsync();
        }
    }

}
