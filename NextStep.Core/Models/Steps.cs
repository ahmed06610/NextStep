using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStep.Core.Models
{
    public class Steps
    {
        [Key]
        public int StepsID { get; set; }

        [ForeignKey("Transaction")]
        public int TransactionID { get; set; }

        [ForeignKey("Department")]
        public int DepartmentID { get; set; }
        public Department Department { get; set; }

        public int StepOrder { get; set; }

        public ICollection<Application> Applications { get; set; }
    }
    // 1 , 2 , 1
    // 1, 5 , 2
}
