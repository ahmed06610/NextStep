using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using NextStep.Core.Const;
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
        private readonly IApplicationTypeService _applicationTypeService;
        private readonly IFileService _fileService;
        private readonly IAuthService _authService;

        public ApplicationsController(
            IApplicationService applicationService,
            IFileService fileService,
            IAuthService authService,
            IApplicationTypeService applicationTypeService)
        {
            _applicationService = applicationService;
            _fileService = fileService;
            _authService = authService;
            _applicationTypeService = applicationTypeService;
        }
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            try
            {
                var application = await _applicationService.GetApplicationDetailsAsync(id, 0);
                if (application == null)
                    return NotFound("Application not found.");

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", application.FileUrl);
                if (!System.IO.File.Exists(filePath))
                    return NotFound("File not found.");

                var contentType = "application/octet-stream";
                new FileExtensionContentTypeProvider().TryGetContentType(filePath, out contentType);
                var fileName = Path.GetFileName(filePath);

                // Set headers to prevent caching
                Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
                Response.Headers["Pragma"] = "no-cache";
                Response.Headers["Expires"] = "0";

                return PhysicalFile(filePath, contentType, fileName);
            }
            catch (Exception ex)
            {
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
            var loggedid = int.Parse(User.FindFirst("LoggedId")?.Value);
            // get the role of the user
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var empid = 0;
            if (userRole != "طالب")
            {
                empid = loggedid ;
            }
            var applicationDetails = await _applicationService.GetApplicationDetailsAsync(id,empid);
            if (applicationDetails == null)
                return NotFound();

            return Ok(applicationDetails);
        }

        [Authorize(Roles = " حسابات علميه,إدارة الدرسات العليا,ذكاء اصطناعي,علوم حاسب,نظم المعلومات")]
        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromForm] CreateApplicationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var employeeId = int.Parse(User.FindFirst("LoggedId")?.Value);
                var departmentId = int.Parse(User.FindFirstValue("DepartmentId"));
                var types = (await _applicationTypeService.GetAllAsync(departmentId)).Select(t=>t.Id);
                if (types.Contains(dto.ApplicationTypeID) == false)
                    return Unauthorized();
                var application = await _applicationService.CreateApplicationAsync(dto, employeeId);
                return Ok(application.ApplicationID);


            }
            catch (Exception  ex)
            {

                return BadRequest(ex.Message);
            }


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
                "حسابات علميه" => true,
                "إدارة الدرسات العليا" => true,
                "ذكاء اصطناعي" => true,
                "علوم حاسب" => true,
                "نظم المعلومات" => true,
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
