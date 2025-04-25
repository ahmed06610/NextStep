using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStep.Core.Models
{
    public class RequiermentsApplicationType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ApplicationTypeId { get; set; }
        public int RequiermentId { get; set; }
        public virtual ApplicationType ApplicationType { get; set; }
        public virtual Requierments Requierment { get; set; }
    }
}
