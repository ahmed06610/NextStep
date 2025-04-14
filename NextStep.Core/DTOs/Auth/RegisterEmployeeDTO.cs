using System.ComponentModel.DataAnnotations;

namespace NextStep.Core.DTOs.Auth
{
    // Employee DTO
    public class RegisterEmployeeDTO : RegisterUserDTO
    {
        [Required]
        public int DepartmentID { get; set; }
    }
}
