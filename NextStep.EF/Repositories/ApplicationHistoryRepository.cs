using NextStep.Core.Interfaces;
using NextStep.Core.Models;
using NextStep.EF.Data;

namespace NextStep.EF.Repositories
{
    public class ApplicationHistoryRepository : BaseRepository<ApplicationHistory>, IApplicationHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationHistoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }

}
