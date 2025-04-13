using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.Models
{
    public class Application
    {
        [Key]
        public int ApplicationID { get; set; }

        [ForeignKey("ApplicationType")]
        public int ApplicationTypeID { get; set; }
        public ApplicationType ApplicationType { get; set; }

        public string Status { get; set; }

        [ForeignKey("CreatedBy")]
        public int CreatedBy { get; set; }
        public Employee CreatedByUser { get; set; }

        public DateTime CreatedDate { get; set; }

        [ForeignKey("Student")]
        public int StudentID { get; set; }
        public Student Student { get; set; }

        public string FileUpload { get; set; }
        public string Notes { get; set; }

        [ForeignKey("Steps")]
        public int StepID { get; set; }
        public Steps Steps { get; set; }
    }
}
