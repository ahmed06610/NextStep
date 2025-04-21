using NextStep.Core.Interfaces;
using NextStep.Core.Models;
using NextStep.EF.Data;

namespace NextStep.EF.Repositories
{
    public class RequiermentsApplicationTypeRepository : BaseRepository<RequiermentsApplicationType>, IRequiermentsApplicationTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public RequiermentsApplicationTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
