using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.Interfaces.Services
{
    public interface IFileService
    {
        Task<string> SaveApplicationFileAsync(IFormFile file, int applicationId);
        void DeleteApplicationFile(string filePath);
        string GetFileFullPath(string relativePath);
    }
}
