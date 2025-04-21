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
        public List<string> Requierment { get; set; } = new List<string>();
    }
}
