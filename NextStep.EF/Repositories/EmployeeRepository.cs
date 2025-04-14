using Microsoft.EntityFrameworkCore;
using NextStep.Core.Interfaces;
using NextStep.Core.Models;
using NextStep.EF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.EF.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Employee> GetByAppUserIdAsync(string id)
       => await _context.Employees.SingleOrDefaultAsync(t => t.UserId == id);
    }

}
