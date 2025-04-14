using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NextStep.Core.DTOs.ApplicationType;
using NextStep.Core.Interfaces.Services;

namespace NextStep.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationTypesController : ControllerBase
    {
        private readonly IApplicationTypeService _service;

        public ApplicationTypesController(IApplicationTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationTypeDTO>>> GetAll()
        {
            var types = await _service.GetAllAsync();
            return Ok(types);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationTypeDTO>> GetById(int id)
        {
            var type = await _service.GetByIdAsync(id);
            if (type == null)
                return NotFound();
            return Ok(type);
        }

        [HttpPost]
        public async Task<ActionResult<ApplicationTypeDTO>> Create([FromBody] CreateApplicationTypeDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdType = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdType.Id }, createdType);
        }

        [HttpPut]
        public async Task<ActionResult<ApplicationTypeDTO>> Update([FromBody] UpdateApplicationTypeDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedType = await _service.UpdateAsync(dto);
                return Ok(updatedType);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
