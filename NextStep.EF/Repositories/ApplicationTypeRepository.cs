using NextStep.Core.Interfaces;
using NextStep.Core.Models;
using NextStep.EF.Data;

namespace NextStep.EF.Repositories
{
    public class ApplicationTypeRepository : BaseRepository<ApplicationType>, IApplicationTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }

}
