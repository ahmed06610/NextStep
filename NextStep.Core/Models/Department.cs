using System.ComponentModel.DataAnnotations;

namespace NextStep.Core.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        public ICollection<Employee> Employees { get; set; }
        public ICollection<Steps> Steps { get; set; }
        public ICollection<ApplicationHistory> ApplicationHistories { get; set; }
    }
}
