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
        public int TotalRequests { get; set; }
        public int CreatedRequests { get; set; }
        public int PendingRequests { get; set; }
        public int DelayedRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
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
}
