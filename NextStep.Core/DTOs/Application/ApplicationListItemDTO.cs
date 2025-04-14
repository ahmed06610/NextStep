namespace NextStep.Core.DTOs.Application
{
    // ApplicationListItemDTO.cs (shared by both inbox and outbox)
    public class ApplicationListItemDTO
    {
        public int ApplicationId { get; set; }
        public string ApplicationType { get; set; }
        public string SendingDepartment { get; set; }
        public DateTime SentDate { get; set; }
        public string Status { get; set; }
        //public List<HistoryItemDTO> History { get; set; }
    }
}
