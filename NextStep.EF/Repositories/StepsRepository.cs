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
    }

}
