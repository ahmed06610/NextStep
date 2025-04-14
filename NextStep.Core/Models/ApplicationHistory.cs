using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NextStep.Core.Models
{
    public class ApplicationHistory
    {
        [Key]
        public int HistoryID { get; set; }

        [ForeignKey("Application")]
        public int ApplicationID { get; set; }
        public virtual Application Application { get; set; }

        public DateTime ActionDate { get; set; }

        [ForeignKey("Department")]
        public int ActionByDeptId { get; set; }
        public virtual Department Department { get; set; }

        public string Action { get; set; }
        public string Notes { get; set; }

    }
}
