using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextStep.Core.Models
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string Naid { get; set; } // university ID maybe?

        public ICollection<Application> Applications { get; set; }
    }
}
