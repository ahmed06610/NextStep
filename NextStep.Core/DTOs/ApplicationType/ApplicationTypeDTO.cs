using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.DTOs.ApplicationType
{
    // ApplicationTypeDTO.cs - For listing/getting
    public class ApplicationTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<RequiermentDTO>? Requierments { get; set; } = new List<RequiermentDTO>();
        public List<StepDTO>? Steps { get; set; } = new List<StepDTO>();
    }

    public class RequiermentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class StepDTO
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public int StepOrder { get; set; }
    }

}
