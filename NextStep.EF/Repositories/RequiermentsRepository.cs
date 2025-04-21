using NextStep.Core.Interfaces;
using NextStep.Core.Models;
using NextStep.EF.Data;

namespace NextStep.EF.Repositories
{
    public class RequiermentsRepository : BaseRepository<Requierments>, IRequiermentsRepository
    {
        private readonly ApplicationDbContext _context;

        public RequiermentsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
       
    }
}
