using ERPHub.Models;
using ERPHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace ERPHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobCardsController : ControllerBase
    {
        private readonly JobCardService _jobCardService;

        public JobCardsController(JobCardService jobCardService)
        {
            _jobCardService = jobCardService;
        }

        [HttpGet("report")]
        public async Task<ActionResult<JobCardReportPageDto>> GetReport(
            [FromQuery] string? employeeId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] int index = 0,
            [FromQuery] int? companyId = null,
            [FromQuery] int? departmentId = null,
            [FromQuery] int? sectionId = null,
            [FromQuery] int? designationId = null,
            [FromQuery] int? lineId = null,
            [FromQuery] int? shiftId = null)
        {
            var page = await _jobCardService.GetJobCardReportPageAsync(new JobCardReportFilter
            {
                EmployeeId = employeeId?.Trim() ?? string.Empty,
                FromDate = fromDate,
                ToDate = toDate,
                CompanyId = companyId,
                DepartmentId = departmentId,
                SectionId = sectionId,
                DesignationId = designationId,
                LineId = lineId,
                ShiftId = shiftId
            }, index);

            if (page == null)
                return NotFound(new { Error = "No employees found matching the selected filters." });

            return Ok(page);
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportExcel(
            [FromQuery] string? employeeId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] int index = 0,
            [FromQuery] int? companyId = null,
            [FromQuery] int? departmentId = null,
            [FromQuery] int? sectionId = null,
            [FromQuery] int? designationId = null,
            [FromQuery] int? lineId = null,
            [FromQuery] int? shiftId = null)
        {
            var page = await _jobCardService.GetJobCardReportPageAsync(new JobCardReportFilter
            {
                EmployeeId = employeeId?.Trim() ?? string.Empty,
                FromDate = fromDate,
                ToDate = toDate,
                CompanyId = companyId,
                DepartmentId = departmentId,
                SectionId = sectionId,
                DesignationId = designationId,
                LineId = lineId,
                ShiftId = shiftId
            }, index);

            if (page == null)
                return NotFound(new { Error = "No data found." });

            var excelService = HttpContext.RequestServices.GetRequiredService<IExportService>();
            var bytes = await excelService.ExportJobCardToExcelAsync(page.Report);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"JobCard_{page.Report.EmployeeId}_{page.Report.FromDate}_{page.Report.ToDate}.xlsx");
        }

        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportPdf(
            [FromQuery] string? employeeId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] int index = 0,
            [FromQuery] int? companyId = null,
            [FromQuery] int? departmentId = null,
            [FromQuery] int? sectionId = null,
            [FromQuery] int? designationId = null,
            [FromQuery] int? lineId = null,
            [FromQuery] int? shiftId = null)
        {
            var page = await _jobCardService.GetJobCardReportPageAsync(new JobCardReportFilter
            {
                EmployeeId = employeeId?.Trim() ?? string.Empty,
                FromDate = fromDate,
                ToDate = toDate,
                CompanyId = companyId,
                DepartmentId = departmentId,
                SectionId = sectionId,
                DesignationId = designationId,
                LineId = lineId,
                ShiftId = shiftId
            }, index);

            if (page == null)
                return NotFound(new { Error = "No data found." });

            var excelService = HttpContext.RequestServices.GetRequiredService<IExportService>();
            var bytes = await excelService.ExportJobCardToPdfAsync(page.Report);
            return File(bytes, "application/pdf", $"JobCard_{page.Report.EmployeeId}_{page.Report.FromDate}_{page.Report.ToDate}.pdf");
        }
    }
}
