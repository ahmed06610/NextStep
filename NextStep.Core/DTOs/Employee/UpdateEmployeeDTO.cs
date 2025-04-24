using System.ComponentModel.DataAnnotations;

namespace NextStep.Core.DTOs.Employee
{
    public class UpdateEmployeeDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
