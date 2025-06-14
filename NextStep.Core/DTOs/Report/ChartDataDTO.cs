using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.DTOs.Report
{

    public class ChartDataDTO<T>
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<T> Data { get; set; } = new List<T>();
    }
    public class GlobalStatsDTO
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int DelayedRequests { get; set; } // Will require a business rule
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
    }
    public class DepartmentStatsDTO
    {
        /// <summary>
        /// The total number of unique applications this department has created or processed.
        /// </summary>
        public int TotalRequests { get; set; }

        public CreatedByDepartmentStatsDTO CreatedByDepartment { get; set; }

        public ReceivedFromOthersStatsDTO ReceivedFromOthers { get; set; }
    }
    public class DepartmentStatusCountDTO
    {
        public int Pending { get; set; }
        public int Delayed { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
    }
    public class DepartmentTimeAnalysisDTO
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<int> ReceivedData { get; set; } = new List<int>();
        public List<int> ProcessedData { get; set; } = new List<int>();
    }
    public class ReceivedFromOthersStatsDTO
    {
        /// <summary>
        /// Applications NOT created by this dept that are currently in THIS dept's inbox.
        /// </summary>
        public int InProgress { get; set; }

        /// <summary>
        /// A subset of InProgress; items in THIS dept's inbox that are delayed.
        /// </summary>
        public int Delayed { get; set; }

        /// <summary>
        /// Applications NOT created by this dept that were given the FINAL approval by THIS dept.
        /// </summary>
        public int AcceptedByDepartment { get; set; }

        /// <summary>
        /// Applications NOT created by this dept that were rejected by THIS dept.
        /// </summary>
        public int RejectedByDepartment { get; set; }
    }
    public class CreatedByDepartmentStatsDTO
    {
        /// <summary>
        /// The total number of applications created by this department.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Applications created by this dept that are currently pending ANYWHERE in the system.
        /// </summary>
        public int InProgress { get; set; }

        /// <summary>
        /// A subset of InProgress; apps created by this dept that are delayed ANYWHERE.
        /// </summary>
        public int Delayed { get; set; }

        /// <summary>
        /// Applications created by this dept that have a final status of "Approved".
        /// </summary>
        public int AcceptedByOthers { get; set; }

        /// <summary>
        /// Applications created by this dept that have a final status of "Rejected".
        /// </summary>
        public int RejectedByOthers { get; set; }
    }
}
