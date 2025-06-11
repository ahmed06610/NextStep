using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextStep.Core.Interfaces.Services;

namespace NextStep.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "اداره التقارير")]

    // [Authorize(Roles = "Admin")] // TODO: Add authorization as needed
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IDepartmentService _departmentService; // To get list of departments

        public ReportsController(IReportService reportService, IDepartmentService departmentService)
        {
            _reportService = reportService;
            _departmentService = departmentService;
        }

        // 1) GET /api/reports/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetGlobalStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _reportService.GetGlobalStatsAsync(startDate, endDate);
            return Ok(result);
        }

        // 2) GET /api/departments (This can live here or in a separate DepartmentsController)
        [HttpGet("/api/departmentss")] // Overriding route to match spec
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _departmentService.GetAllAsync();
            // Map to the simple format requested if needed, or adjust the DTO
            var simpleDepartments = departments.Select(d => new { id = d.Id, name = d.DepartmentName });
            return Ok(simpleDepartments);
        }

        // 3) GET /api/reports/requests/time-analysis
        [HttpGet("requests/time-analysis")]
        public async Task<IActionResult> GetTimeAnalysis([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _reportService.GetAverageProcessingTimeByDepartmentAsync(startDate, endDate);
            return Ok(result);
        }

        // 4) GET /api/reports/departments/requests-count
        [HttpGet("departments/requests-count")]
        public async Task<IActionResult> GetDepartmentRequestCounts([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _reportService.GetRequestCountByDepartmentAsync(startDate, endDate);
            return Ok(result);
        }

        // GET /api/reports/requests/created
        [HttpGet("requests/created")]
        public async Task<IActionResult> GetCreatedRequests([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            // Call the new dynamic method
            var result = await _reportService.GetCreatedRequestsOverTimeAsync(startDate, endDate);
            return Ok(result);
        }

        // 6) GET /api/reports/departments/status?status=delayed|rejected|approved|pending
        [HttpGet("departments/status")]
        public async Task<IActionResult> GetDepartmentStatusCounts([FromQuery] string status, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("Status query parameter is required.");
            }
            var result = await _reportService.GetDepartmentStatusCountsAsync(status, startDate, endDate);
            return Ok(result);
        }


        // --- APIs for Department Details ---

        // GET /api/departments/{id}
        [HttpGet("/api/departments/{id}")]
        public async Task<IActionResult> GetDepartmentDetails(int id)
        {
            var department = (await _departmentService.GetAllAsync()).FirstOrDefault(d => d.Id == id);
            if (department == null) return NotFound();
            return Ok(new { id = department.Id, name = department.DepartmentName });
        }

        // GET /api/departments/{id}/stats
        [HttpGet("/api/departments/{id}/stats")]
        public async Task<IActionResult> GetDepartmentStats(int id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _reportService.GetDepartmentStatsAsync(id, startDate, endDate);
            return Ok(result);
        }

        // GET /api/departments/{id}/requests/status
        [HttpGet("/api/departments/{id}/requests/status")]
        public async Task<IActionResult> GetDepartmentRequestStatus(int id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _reportService.GetDepartmentRequestStatusCountsAsync(id, startDate, endDate);
            return Ok(result);
        }

        // GET /api/departments/{id}/processing-time
        [HttpGet("/api/departments/{id}/processing-time")]
        public async Task<IActionResult> GetDepartmentProcessingTime(int id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _reportService.GetAverageProcessingTimeByRequestTypeAsync(id, startDate, endDate);
            return Ok(result);
        }

        // GET /api/departments/{id}/requests-count
        [HttpGet("/api/departments/{id}/requests-count")]
        public async Task<IActionResult> GetDepartmentRequestCountByType(int id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _reportService.GetRequestCountByRequestTypeAsync(id, startDate, endDate);
            return Ok(result);
        }

        // GET /api/departments/{id}/time-analysis
        [HttpGet("/api/departments/{id}/time-analysis")]
        public async Task<IActionResult> GetDepartmentMonthlyAnalysis(int id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            // Call the new dynamic method
            var result = await _reportService.GetDepartmentTimeAnalysisAsync(id, startDate, endDate);
            return Ok(result);
        }

        // GET /api/departments/{id}/rejection-reasons
        [HttpGet("/api/departments/{id}/rejection-reasons")]
        public async Task<IActionResult> GetDepartmentRejectionReasons(int id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _reportService.GetRejectionReasonsAsync(id, startDate, endDate);
            return Ok(result);
        }
    }
}
