using System.ComponentModel.DataAnnotations;

namespace NextStep.Core.DTOs.Auth
{
    // Student DTO
    public class RegisterStudentDTO : RegisterUserDTO
    {
        [Required]
        public string Naid { get; set; } // University ID
        [Required, Phone]
        public string PhoneNumber { get; set; }
    }
}
