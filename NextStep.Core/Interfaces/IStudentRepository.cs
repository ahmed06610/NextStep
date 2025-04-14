using NextStep.Core.Models;

namespace NextStep.Core.Interfaces
{
    public interface IStudentRepository : IBaseRepository<Student>
    {
        Task<Student> GetByAppUserIdAsync(string id);
        Task<Student> GetByNaidAsync(string naid);


    }

}
