using System.ComponentModel.DataAnnotations;

namespace NextStep.Core.Models
{
    public class ApplicationType
    {
        [Key]
        public int ApplicationTypeID { get; set; }

        public string ApplicationTypeName { get; set; }
        public string Description { get; set; }

        public ICollection<Application> Applications { get; set; }
    }
}
