using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
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
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(
            [FromQuery] string? searchQuery = null,
            [FromQuery] string? employeeId = null,
            [FromQuery] string? company = null,
            [FromQuery] string? department = null,
            [FromQuery] string? section = null,
            [FromQuery] string? designation = null,
            [FromQuery] string? line = null,
            [FromQuery] string? status = null,
            [FromQuery] string? year = null)
        {
            List<Employee> employees;

            if (searchQuery != null || employeeId != null || company != null ||
                department != null || section != null || designation != null ||
                line != null || status != null || year != null)
            {
                var filter = new EmployeeFilter
                {
                    SearchQuery = searchQuery,
                    EmployeeId = employeeId,
                    Company = company,
                    Department = department,
                    Section = section,
                    Designation = designation,
                    Line = line,
                    Status = status,
                    Year = year
                };
                employees = await _erpService.GetFilteredEmployeesAsync(filter);
            }
            else
            {
                employees = await _erpService.GetEmployeesAsync();
            }

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

            employee.Department = null;
            employee.Section = null;
            employee.Designation = null;
            employee.Line = null;
            employee.Shift = null;
            employee.Company = null;

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

            // Null out navigation properties so model validation doesn't
            // fail on their [Required] fields (e.g. NameBn)
            employee.Department = null;
            employee.Section = null;
            employee.Designation = null;
            employee.Line = null;
            employee.Shift = null;
            employee.Company = null;

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

        // GET: api/employees/import/template
        [HttpGet("import/template")]
        public IActionResult DownloadImportTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Employees Template");
            
            var headers = new[]
            {
                "Employee ID", "Employee Name", "Punch Number", "Mobile No", "Email",
                "Company", "Department", "Section", "Designation", "Line", "Shift",
                "Joining Date", "Basic Salary", "Gross Salary", "Gender", "Date of Birth",
                "Employee Status", "Employee Type", "Overtime Status"
            };

            for (int col = 1; col <= headers.Length; col++)
            {
                worksheet.Cells[1, col].Value = headers[col - 1];
                worksheet.Cells[1, col].Style.Font.Bold = true;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var bytes = package.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "employee_import_template.xlsx");
        }

        // POST: api/employees/import
        [HttpPost("import")]
        public async Task<ActionResult<ImportResultDto>> ImportEmployees(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file was uploaded.");

            var extension = Path.GetExtension(file.FileName);
            if (!extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only Excel files (.xlsx) are supported.");

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                var result = await _erpService.ImportEmployeesFromExcelAsync(stream);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred during import: {ex.Message}");
            }
        }

        // GET: api/employees/import-organogram/template
        [HttpGet("import-organogram/template")]
        public IActionResult DownloadOrganogramTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Organogram Template");
            
            var headers = new[]
            {
                "Department (EN)", "Department (BN)", "Section (EN)", "Section (BN)",
                "Designation (EN)", "Designation (BN)", "Line (EN)", "Line (BN)"
            };

            for (int col = 1; col <= headers.Length; col++)
            {
                worksheet.Cells[1, col].Value = headers[col - 1];
                worksheet.Cells[1, col].Style.Font.Bold = true;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var bytes = package.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "organogram_import_template.xlsx");
        }

        // POST: api/employees/import-organogram
        [HttpPost("import-organogram")]
        public async Task<ActionResult<ImportResultDto>> ImportOrganogram(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file was uploaded.");

            var extension = Path.GetExtension(file.FileName);
            if (!extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only Excel files (.xlsx) are supported.");

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                var result = await _erpService.ImportOrganogramFromExcelAsync(stream);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred during import: {ex.Message}");
            }
        }
    }
}
