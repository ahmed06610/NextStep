using System.ComponentModel.DataAnnotations;

namespace NextStep.Core.DTOs.ApplicationType
{
    // CreateApplicationTypeDTO.cs - For creation
    public class CreateApplicationTypeDTO
    {
        [Required]
        public string ApplicationTypeName { get; set; }

        public string Description { get; set; }
    }
}
