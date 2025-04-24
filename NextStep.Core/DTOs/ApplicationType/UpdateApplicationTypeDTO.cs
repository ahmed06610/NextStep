using System.ComponentModel.DataAnnotations;

namespace NextStep.Core.DTOs.ApplicationType
{
    // UpdateApplicationTypeDTO.cs - For updates
    public class UpdateApplicationTypeDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string ApplicationTypeName { get; set; }

        public string Description { get; set; }

        public List<CreateStepsDTO> Steps { get; set; } = new List<CreateStepsDTO>();

        public List<CreateRequiermentDTO> Requierments { get; set; } = new List<CreateRequiermentDTO>();
    }

}
