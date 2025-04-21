namespace NextStep.Core.Models
{
    public class RequiermentsApplicationType
    {
        public int Id { get; set; }
        public int ApplicationTypeId { get; set; }
        public int RequiermentId { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public Requierments Requierment { get; set; }
    }
}
