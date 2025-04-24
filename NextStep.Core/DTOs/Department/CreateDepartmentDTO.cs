using System.ComponentModel.DataAnnotations;

namespace NextStep.Core.DTOs.Department
{
    public class CreateDepartmentDTO
    {
        [Required]
        public string DepartmentName { get; set; }
    }
}
