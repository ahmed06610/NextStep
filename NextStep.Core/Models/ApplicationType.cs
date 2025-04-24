using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextStep.Core.Models
{
    public class ApplicationType
    {
        [Key]
        public int ApplicationTypeID { get; set; }

        public string ApplicationTypeName { get; set; }
        public string Description { get; set; }
        [ForeignKey("Department")]
        public int? CreatedByDeptId { get; set; }
        public virtual Department Department { get; set; }

        public virtual ICollection<Steps> Steps { get; set; }   
        public virtual ICollection<Application> Applications { get; set; }
        public virtual ICollection<RequiermentsApplicationType> Requierments { get; set; }
    }
}
