using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using NextStep.Core.DTOs.Application;
using NextStep.Core.Interfaces.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NextStep.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IFileService _fileService;
        private readonly IAuthService _authService;

        public ApplicationsController(
            IApplicationService applicationService,
            IFileService fileService,
            IAuthService authService)
        {
            _applicationService = applicationService;
            _fileService = fileService;
            _authService = authService;
        }
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            try
            {
                // Retrieve the application record
                var application = await _applicationService.GetApplicationDetailsAsync(id);
                if (application == null)
                    return NotFound("Application not found.");

                // Get the file path from the FileUpload property
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", application.FileUrl);

                // Check if the file exists
                if (!System.IO.File.Exists(filePath))
                    return NotFound("File not found.");

                // Get the file's content type
                var contentType = "application/octet-stream"; // Default content type
                new FileExtensionContentTypeProvider().TryGetContentType(filePath, out contentType);

                // Return the file as a response
                var fileName = Path.GetFileName(filePath);
                return PhysicalFile(filePath, contentType, fileName);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in DownloadFile: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet("GetAppsForStudent")]
        [Authorize( Roles ="طالب")]
        public async Task<IActionResult> GetAppsForStudent()
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var StudentId = int.Parse(User.FindFirst("LoggedId")?.Value);
            var apps = await _applicationService.GetApplicationsForStuent(StudentId);
            return Ok(apps);


        }
        [Authorize]
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetApplicationDetails(int id)
        {
            var applicationDetails = await _applicationService.GetApplicationDetailsAsync(id);
            if (applicationDetails == null)
                return NotFound();

            return Ok(applicationDetails);
        }

        [Authorize(Roles = "موظف حسابات علميه, موظف إدارة الدرسات العليا, موظف ذكاء اصطناعي, موظف علوم حاسب, موظف نظم المعلومات")]
        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromForm] CreateApplicationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employeeId = int.Parse(User.FindFirst("LoggedId")?.Value);
            var application = await _applicationService.CreateApplicationAsync(dto, employeeId);

            return Ok();
        }

        [Authorize]
        [HttpPost("approve")]
        public async Task<IActionResult> ApproveApplication([FromForm] ApplicationActionDTO dto)
        {
            var employeeId = int.Parse(User.FindFirst("LoggedId")?.Value);
            var departmentId = int.Parse(User.FindFirstValue("DepartmentId")); // You'll need to add this claim

            var result = await _applicationService.ApproveApplicationAsync(dto, employeeId, departmentId);
            if (!result)
                return BadRequest("Unable to approve application");

            return Ok();
        }

        [Authorize]
        [HttpPost("reject")]
        public async Task<IActionResult> RejectApplication([FromForm] ApplicationActionDTO dto)
        {
            var employeeId = int.Parse(User.FindFirst("LoggedId")?.Value);
            var departmentId = int.Parse(User.FindFirstValue("DepartmentId")); // You'll need to add this claim

            var result = await _applicationService.RejectApplicationAsync(dto, employeeId, departmentId);
            if (!result)
                return BadRequest("Unable to reject application");

            return Ok();
        }
        [HttpGet("inbox")]
        public async Task<IActionResult> GetInbox(
    [FromQuery] string search = null,
    [FromQuery] int? requestType = null,
    [FromQuery] string status = null,
    [FromQuery] int page = 1,
    [FromQuery] int limit = 10)
        {
            var departmentId = int.Parse(User.FindFirstValue("DepartmentId"));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            // Check if department creates orders
            bool isOrderCreatingDepartment = userRole switch
            {
                "موظف حسابات علميه" => true,
                "موظف إدارة الدرسات العليا" => true,
                "موظف ذكاء اصطناعي" => true,
                "موظف علوم حاسب" => true,
                "موظف نظم المعلومات" => true,
                _ => false
            };

            var result = await _applicationService.GetInboxApplicationsAsync(
                departmentId,
                isOrderCreatingDepartment,
                search,
                requestType,
                status,
                page,
                limit);

            return Ok(result);
        }

        [HttpGet("outbox")]
        public async Task<IActionResult> GetOutbox(
            [FromQuery] string search = null,
            [FromQuery] int? requestType = null,
            [FromQuery] string status = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var departmentId = int.Parse(User.FindFirstValue("DepartmentId"));

            var result = await _applicationService.GetOutboxApplicationsAsync(
                departmentId,
                search,
                requestType,
                status,
                page,
                limit);

            return Ok(result);
        }


        /*  [HttpGet("{id}")]
          public async Task<IActionResult> GetApplication(int id)
          {
              var application = await _applicationService.GetByIdWithDetailsAsync(id);
              if (application == null)
                  return NotFound();

              return Ok(application);
          }*/
    }
}
