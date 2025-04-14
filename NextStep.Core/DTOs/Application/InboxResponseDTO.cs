namespace NextStep.Core.DTOs.Application
{
    // InboxResponseDTO.cs
    public class InboxResponseDTO
    {
        public InboxSummaryDTO Summary { get; set; }
        public List<ApplicationListItemDTO> Applications { get; set; }
    }
}
