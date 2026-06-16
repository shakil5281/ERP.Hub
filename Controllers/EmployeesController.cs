using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ERPHub.Models;
using ERPHub.Services;

namespace ERPHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IErpService _erpService;

        public EmployeesController(IErpService erpService)
        {
            _erpService = erpService ?? throw new ArgumentNullException(nameof(erpService));
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var employees = await _erpService.GetEmployeesAsync();
            return Ok(employees);
        }

        // GET: api/employees/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _erpService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound($"Employee with ID {id} not found.");
            return Ok(employee);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee([FromBody] Employee employee)
        {
            if (employee == null)
                return BadRequest("Invalid employee data.");

            await _erpService.AddEmployeeAsync(employee);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        // PUT: api/employees/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] Employee employee)
        {
            if (employee == null || id != employee.Id)
                return BadRequest("Employee ID mismatch.");

            var existing = await _erpService.GetEmployeeByIdAsync(id);
            if (existing == null)
                return NotFound($"Employee with ID {id} not found.");

            await _erpService.UpdateEmployeeAsync(employee);
            return NoContent();
        }

        // DELETE: api/employees/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var existing = await _erpService.GetEmployeeByIdAsync(id);
            if (existing == null)
                return NotFound($"Employee with ID {id} not found.");

            await _erpService.DeleteEmployeeAsync(id);
            return Ok($"Employee with ID {id} has been deleted.");
        }
    }
}
