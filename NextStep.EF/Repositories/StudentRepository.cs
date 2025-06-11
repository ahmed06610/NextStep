using Microsoft.EntityFrameworkCore;
using NextStep.Core.Interfaces;
using NextStep.Core.Models;
using NextStep.EF.Data;

namespace NextStep.EF.Repositories
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Student> GetByAppUserIdAsync(string id)
        => await _context.Students.SingleOrDefaultAsync(t => t.UserId == id);
        public async Task<Student> GetByNaidAsync(string naid)
        {
            return await _context.Students.Include(s=>s.User)
                .FirstOrDefaultAsync(s => s.Naid == naid);
        }
    }

}
