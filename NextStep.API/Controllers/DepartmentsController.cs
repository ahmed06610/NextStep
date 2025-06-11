using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextStep.Core.DTOs.Department;
using NextStep.Core.Interfaces.Services;

namespace NextStep.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DepartmentsController(IDepartmentService departmentService, RoleManager<IdentityRole> roleManager)
        {
            _departmentService = departmentService;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDTO>>> GetAll()
        {
            var departments = await _departmentService.GetAllAsync();
            return Ok(departments);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var department = await _departmentService.CreateAsync(dto);

            // Add role for the department
            var roleName = $"موظف {dto.DepartmentName}";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            return CreatedAtAction(nameof(GetAll), new { id = department.Id }, department);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _departmentService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
