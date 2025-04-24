using System.ComponentModel.DataAnnotations;

namespace NextStep.Core.DTOs.ApplicationType
{
    // CreateApplicationTypeDTO.cs - For creation
    public class CreateApplicationTypeDTO
    {
        [Required]
        public string ApplicationTypeName { get; set; }

        public string Description { get; set; }
        public List<CreateStepsDTO> createStepsDTOs { get; set; } = new List<CreateStepsDTO>();
        public List<CreateRequiermentDTO> createRequiermentDTOs { get; set; } = new List<CreateRequiermentDTO>();

    }
    public class CreateStepsDTO
    {
      
        [Required]
        public int DepartmentId { get; set; }
        [Required]
        public int StepOrder { get; set; }

    }

    public class CreateRequiermentDTO
    {
        [Required]
        public string RequiermentName { get; set; }
    }
}
