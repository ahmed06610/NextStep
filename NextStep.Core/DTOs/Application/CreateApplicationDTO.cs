using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.DTOs.Application
{
    // CreateApplicationDTO.cs
    public class CreateApplicationDTO
    {
        [Required]
        public int ApplicationTypeID { get; set; }

        [Required]
        public string StudentNaid { get; set; }

        [Required]
        public string StudentName { get; set; }

        [Required]
        public string StudentPhone { get; set; }

        [Required]
        public IFormFile Attachment { get; set; }

        public string Notes { get; set; }
    }

    // ApplicationActionDTO.cs (for approve/reject)
    public class ApplicationActionDTO
    {
        [Required]
        public int ApplicationID { get; set; }

        public IFormFile? Attachment { get; set; }

        public string Notes { get; set; }
    }
}
