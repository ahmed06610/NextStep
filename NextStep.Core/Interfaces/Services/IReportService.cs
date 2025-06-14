using NextStep.Core.DTOs.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.Interfaces.Services
{
    // In Core/Interfaces/Services/IReportService.cs
    public interface IReportService
    {
        #region General Reports

        /// <summary>
        /// Gets the main dashboard statistics (Total, Approved, Rejected, Pending, Delayed).
        /// </summary>
        Task<GlobalStatsDTO> GetGlobalStatsAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Gets the number of currently pending requests for each department.
        /// </summary>
        Task<ChartDataDTO<int>> GetRequestCountByDepartmentAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Gets a count of applications for a specific status (Approved, Rejected, Pending, Delayed), grouped by department.
        /// </summary>
        Task<ChartDataDTO<int>> GetDepartmentStatusCountsAsync(string status, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Gets the average time (in days) that applications spend in each department before being processed.
        /// </summary>
        Task<ChartDataDTO<double>> GetAverageProcessingTimeByDepartmentAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Gets a count of newly created applications over a dynamic time period (Day, Month, or Year).
        /// </summary>
        Task<ChartDataDTO<int>> GetCreatedRequestsOverTimeAsync(DateTime? startDate, DateTime? endDate);

        #endregion


        #region Department-Specific Reports

        /// <summary>
        /// Gets detailed statistics for a single department.
        /// </summary>
        Task<DepartmentStatsDTO> GetDepartmentStatsAsync(int departmentId, DateTime? startDate, DateTime? endDate);
        /// <summary>
        /// Gets a simple count of applications by status for a single department.
        /// </summary>
        Task<DepartmentStatusCountDTO> GetDepartmentRequestStatusCountsAsync(int departmentId, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// For a specific department, gets the number of currently pending requests, grouped by Application Type.
        /// </summary>
        Task<ChartDataDTO<int>> GetRequestCountByRequestTypeAsync(int departmentId, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// For a specific department, gets the average processing time for each Application Type it handles.
        /// </summary>
        Task<ChartDataDTO<double>> GetAverageProcessingTimeByRequestTypeAsync(int departmentId, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// For a specific department, gets a time-series chart of applications received vs. applications processed.
        /// </summary>
        Task<DepartmentTimeAnalysisDTO> GetDepartmentTimeAnalysisAsync(int departmentId, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// For a specific department, gets the counts of different rejection reasons based on history notes.
        /// </summary>
        Task<ChartDataDTO<int>> GetRejectionReasonsAsync(int departmentId, DateTime? startDate, DateTime? endDate);

        #endregion
    }
}
