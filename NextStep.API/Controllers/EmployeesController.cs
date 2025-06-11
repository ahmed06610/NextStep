using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStep.Core.DTOs.Employee;
using NextStep.Core.Interfaces.Services;

namespace NextStep.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _employeeService.UpdateAsync(id, dto);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _employeeService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound();
            return Ok(employee);
        }
    }
}
