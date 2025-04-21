using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.DTOs.Application
{
    public class ApplicationDetailsDTO
    {
        public int ApplicationId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDepartment { get; set; }
        public string Notes { get; set; }
        public string FileUrl { get; set; }
        public List<HistoryItemDTO> History { get; set; }
        public List<ApplicationStepDTO> Steps { get; set; }
        public string Statue { get; set; }
    }

    public class ApplicationStepDTO
    {
        public string DepartmentName { get; set; }
        public int StepOrder { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCurrent { get; set; }
    }
}
