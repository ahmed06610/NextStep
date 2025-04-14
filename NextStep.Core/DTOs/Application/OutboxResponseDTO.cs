namespace NextStep.Core.DTOs.Application
{
    // OutboxResponseDTO.cs
    public class OutboxResponseDTO
    {
        public OutboxSummaryDTO Summary { get; set; }
        public List<ApplicationListItemDTO> Applications { get; set; }
    }
}
