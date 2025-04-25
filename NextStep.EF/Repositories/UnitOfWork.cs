using Microsoft.EntityFrameworkCore.Storage;
using NextStep.Core.Interfaces;
using NextStep.EF.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.EF.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Employee = new EmployeeRepository(_context);
            Student = new StudentRepository(_context);
            Department = new DepartmentRepository(_context);
            Application = new ApplicationRepository(_context);
            ApplicationHistory = new ApplicationHistoryRepository(_context);
            ApplicationType = new ApplicationTypeRepository(_context);
            Steps = new StepsRepository(_context);
            Requierments = new RequiermentsRepository(_context);
            RequiermentsApplicationType= new RequiermentsApplicationTypeRepository(_context);

        }

        public IEmployeeRepository Employee { get; private set; }
        public IStudentRepository Student { get; private set; }
        public IDepartmentRepository Department { get; private set; }
        public IApplicationRepository Application { get; private set; }
        public IApplicationHistoryRepository ApplicationHistory { get; private set; }
        public IApplicationTypeRepository ApplicationType { get; private set; }
        public IStepsRepository Steps { get; private set; }
        public IRequiermentsRepository Requierments { get; private set; }
        public IRequiermentsApplicationTypeRepository RequiermentsApplicationType { get; private set; }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}


