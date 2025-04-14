namespace NextStep.Core.DTOs.Application
{
    // OutboxSummaryDTO.cs
    public class OutboxSummaryDTO
    {
        public int TotalApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public int InProgressApplications { get; set; }
    }
}
