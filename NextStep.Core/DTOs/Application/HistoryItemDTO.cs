namespace NextStep.Core.DTOs.Application
{
    // HistoryItemDTO.cs
    public class HistoryItemDTO
    {
        public DateTime InDate { get; set; }
        public DateTime? OutDate { get; set; }
        public string Action { get; set; }
        public string Department { get; set; }
        public string Notes { get; set; }
    }
}
