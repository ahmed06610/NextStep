using NextStep.Core.Models;

namespace NextStep.Core.Interfaces
{
    public interface IStepsRepository : IBaseRepository<Steps>
    {
        Task<Steps> GetInitialStepByApplicationType(int applicationTypeId);
        Task<Steps> GetNextStepAsync(int applicationTypeId, int currentStepOrder);
             

    }

}
