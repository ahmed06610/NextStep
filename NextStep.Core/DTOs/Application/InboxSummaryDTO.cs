using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.DTOs.Application
{
    // InboxSummaryDTO.cs
    public class InboxSummaryDTO
    {
        public int TotalApplications { get; set; }
        public int NewApplications { get; set; }
        public int? AnsweredApplications { get; set; } // Nullable for departments that don't create orders
    }
}
