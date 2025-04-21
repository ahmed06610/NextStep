using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IEmployeeRepository Employee { get; }
        IStudentRepository Student { get; }
        IDepartmentRepository Department { get; }
        IApplicationRepository Application { get; }
        IApplicationHistoryRepository ApplicationHistory { get; }
        IApplicationTypeRepository ApplicationType { get; }
        IStepsRepository Steps { get; }
        IRequiermentsRepository Requierments { get; }
        IRequiermentsApplicationTypeRepository RequiermentsApplicationType { get; }



        Task<int> CompleteAsync();
        Task RollbackAsync();
        Task CommitAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
