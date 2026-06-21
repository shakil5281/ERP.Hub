using Microsoft.AspNetCore.Mvc;
using ERPHub.Models;
using ERPHub.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ERPHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveApplicationsController : ControllerBase
    {
        private readonly IErpService _erpService;
        public LeaveApplicationsController(IErpService erpService) => _erpService = erpService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveApplication>>> GetLeaveApplications(
            [FromQuery] int? month,
            [FromQuery] int? year,
            [FromQuery] string? status)
            => Ok(await _erpService.GetLeaveApplicationsAsync(month, year, status));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LeaveApplication>> GetLeaveApplication(int id)
        {
            var app = await _erpService.GetLeaveApplicationByIdAsync(id);
            return app == null ? NotFound() : Ok(app);
        }

        [HttpPost]
        public async Task<ActionResult<LeaveApplication>> PostLeaveApplication([FromBody] LeaveApplication application)
        {
            if (application == null) return BadRequest();
            await _erpService.AddLeaveApplicationAsync(application);
            return CreatedAtAction(nameof(GetLeaveApplication), new { id = application.Id }, application);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutLeaveApplication(int id, [FromBody] LeaveApplication application)
        {
            if (application == null || id != application.Id) return BadRequest();
            var existing = await _erpService.GetLeaveApplicationByIdAsync(id);
            if (existing == null) return NotFound();
            await _erpService.UpdateLeaveApplicationAsync(application);
            return NoContent();
        }

        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> ApproveLeaveApplication(int id, [FromBody] ApproveRequest req)
        {
            var existing = await _erpService.GetLeaveApplicationByIdAsync(id);
            if (existing == null) return NotFound();
            await _erpService.ApproveLeaveApplicationAsync(id, req?.ApprovedBy ?? "HR Manager");
            return Ok(new { message = "Leave application approved." });
        }

        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> RejectLeaveApplication(int id, [FromBody] RejectRequest req)
        {
            var existing = await _erpService.GetLeaveApplicationByIdAsync(id);
            if (existing == null) return NotFound();
            await _erpService.RejectLeaveApplicationAsync(id, req?.RejectedBy ?? "HR Manager", req?.Reason ?? "");
            return Ok(new { message = "Leave application rejected." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLeaveApplication(int id)
        {
            var existing = await _erpService.GetLeaveApplicationByIdAsync(id);
            if (existing == null) return NotFound();
            await _erpService.DeleteLeaveApplicationAsync(id);
            return Ok();
        }

        [HttpGet("{id:int}/pdf")]
        public async Task<IActionResult> DownloadLeavePdf(int id)
        {
            var app = await _erpService.GetLeaveApplicationByIdAsync(id);
            if (app == null) return NotFound();

            var employees = await _erpService.GetEmployeesAsync();
            var employee = employees.FirstOrDefault(e => e.EmployeeId.Equals(app.EmployeeId, StringComparison.OrdinalIgnoreCase));

            var companies = await _erpService.GetCompaniesAsync();
            var company = companies.FirstOrDefault(c => c.Id == employee?.CompanyId);

            var allLeaveTypes = await _erpService.GetLeaveTypesAsync();
            var allLeaveApps = await _erpService.GetLeaveApplicationsAsync(null, DateTime.Now.Year, "Approved");
            var myLeaves = allLeaveApps.Where(a => a.EmployeeId == app.EmployeeId).ToList();

            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    page.Content().Column(col => 
                    {
                        // HEADER
                        var companyName = company?.CompanyNameEn ?? "ERP HUB GARMENTS LTD.";
                        var companyAddress = company?.AddressEn ?? "Corporate Office / Factory Address";
                        col.Item().Row(r => 
                        {
                            r.RelativeItem().Column(c => 
                            {
                                c.Item().AlignCenter().Text(companyName).FontSize(16).Bold();
                                c.Item().AlignCenter().Text(companyAddress).FontSize(10);
                                c.Item().PaddingTop(5).AlignCenter().Text("LEAVE APPLICATION FORM").FontSize(14).Bold().Underline();
                            });
                        });
                        col.Item().PaddingTop(10).AlignRight().Text($"Date: {app.CreatedAt:dd MMM yyyy}");

                        col.Item().PaddingTop(15).Column(appCol => 
                        {
                            appCol.Spacing(10);

                            // Name & Designation
                            appCol.Item().Row(r => {
                                r.RelativeItem(3).Text(t => { t.Span("Name: "); t.Span(app.EmployeeName).Underline(); });
                                r.RelativeItem(2).Text(t => { t.Span("Designation: "); t.Span(app.Designation?.NameEn ?? "-").Underline(); });
                            });

                            // Section/Line & Card No
                            appCol.Item().Row(r => {
                                r.RelativeItem(3).Text(t => { t.Span("Section/Line: "); t.Span($"{employee?.Section?.NameEn} / {employee?.Line?.NameEn}").Underline(); });
                                r.RelativeItem(2).Text(t => { t.Span("Card No: "); t.Span(app.EmployeeId).Underline(); });
                            });

                            // Leave Reason & Dates
                            appCol.Item().Row(r => {
                                r.RelativeItem(2).Text(t => { t.Span("Reason for Leave: "); t.Span(app.Reason).Underline(); });
                                r.RelativeItem(3).Text(t => { 
                                    t.Span("Leave Date: "); 
                                    t.Span("From ").Bold(); t.Span(app.LeaveDate.ToString("dd MMM yyyy")).Underline();
                                    t.Span(" To ").Bold(); t.Span(app.EndDate.ToString("dd MMM yyyy")).Underline();
                                });
                            });

                            // Total days requested
                            appCol.Item().Text(t => {
                                t.Span("I am requesting ");
                                t.Span(app.TotalDays.ToString()).Underline();
                                t.Span(" days of leave to be granted.");
                            });

                            // Address during leave
                            appCol.Item().Text(t => {
                                t.Span("Address during leave: ");
                                t.Span("...................................................................................................................................").FontColor(Colors.Grey.Medium);
                            });

                            // Phone & Applicant Signature
                            appCol.Item().PaddingTop(15).Row(r => {
                                r.RelativeItem().Text(t => { t.Span("Phone: "); t.Span(employee?.MobileNo ?? "-").Underline(); });
                                r.RelativeItem().AlignRight().Text("Applicant Signature");
                            });
                        });

                        col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                        // OFFICE USE SECTION
                        col.Item().AlignCenter().Text("This part will be filled by the office").Bold();
                        col.Item().PaddingTop(10).Column(officeCol => 
                        {
                            officeCol.Spacing(10);
                            officeCol.Item().Row(r => {
                                r.RelativeItem().Text(t => { t.Span("Joining Date: "); t.Span(employee?.JoiningDate.ToString("dd MMM yyyy") ?? "-").Underline(); });
                                r.RelativeItem().Text(t => { t.Span("Leave calculation period: "); t.Span("_______ to _______").FontColor(Colors.Grey.Medium); });
                            });

                            // Leave details table horizontally
                            officeCol.Item().Row(r => 
                            {
                                r.AutoItem().PaddingRight(10).Column(c => {
                                    c.Item().Height(25); // Spacer for header row
                                    c.Item().Height(20).AlignMiddle().Text("Entitled :");
                                    c.Item().Height(20).AlignMiddle().Text("Availed :");
                                    c.Item().Height(20).AlignMiddle().Text("Balance :");
                                });

                                foreach (var lt in allLeaveTypes)
                                {
                                    var consumed = myLeaves.Where(l => l.LeaveType == lt.Name).Sum(l => l.TotalDays);
                                    var balance = Math.Max(0, lt.MaxDaysPerYear - consumed);

                                    r.RelativeItem().PaddingRight(5).Column(c => {
                                        c.Item().Height(25).AlignMiddle().AlignCenter().Text(lt.Name).FontSize(9).Bold();
                                        c.Item().Height(20).Border(1).AlignMiddle().AlignCenter().Text(lt.MaxDaysPerYear.ToString());
                                        c.Item().Height(20).Border(1).AlignMiddle().AlignCenter().Text(consumed.ToString());
                                        c.Item().Height(20).Border(1).AlignMiddle().AlignCenter().Text(balance.ToString());
                                    });
                                }
                            });

                            // ____ days leave granted.
                            officeCol.Item().PaddingTop(5).Text(t => {
                                t.Span("_______").Underline().FontColor(Colors.Grey.Medium);
                                t.Span(" days leave granted.");
                            });

                            // Signatures
                            officeCol.Item().PaddingTop(40).Row(r => {
                                r.RelativeItem().AlignCenter().Text("HR Dept").FontSize(9);
                                r.RelativeItem().AlignCenter().Text("Incharge").FontSize(9);
                                r.RelativeItem().AlignCenter().Text("APM/PM/QC M").FontSize(9);
                                r.RelativeItem().AlignCenter().Text("Head of Dept").FontSize(9);
                                r.RelativeItem().AlignCenter().Text("Head of Dept (Admin)").FontSize(9);
                                r.RelativeItem().AlignCenter().Text("GM").FontSize(9);
                            });
                        });

                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);

                        // TEAR-OFF SECTION
                        col.Item().PaddingTop(10).Column(tearCol => 
                        {
                            tearCol.Item().AlignCenter().Text(companyName).Bold().FontSize(12);
                            tearCol.Item().AlignCenter().Text("Joining report after leave").FontSize(10);

                            tearCol.Item().PaddingTop(15).Row(r => {
                                r.RelativeItem(3).Text(t => { t.Span("Name: "); t.Span("..........................................").FontColor(Colors.Grey.Medium); });
                                r.RelativeItem(2).Text(t => { t.Span("Card No: "); t.Span("................................").FontColor(Colors.Grey.Medium); });
                                r.RelativeItem(2).Text(t => { t.Span("Issue Date: "); t.Span("...........................").FontColor(Colors.Grey.Medium); });
                            });

                            tearCol.Item().PaddingTop(10).Row(r => {
                                r.RelativeItem().Text(t => { t.Span("Joining date as per granted leave: "); t.Span("................................................................").FontColor(Colors.Grey.Medium); });
                            });

                            tearCol.Item().PaddingTop(10).Row(r => {
                                r.RelativeItem().Text(t => { t.Span("Actual joining date: "); t.Span("..............................................................................").FontColor(Colors.Grey.Medium); });
                            });

                            tearCol.Item().PaddingTop(30).Row(r => {
                                r.RelativeItem().Text("Applicant Signature").FontSize(9);
                                r.RelativeItem(2).AlignCenter().Text("This part should be submitted to admin dept when joining after leave.").FontSize(8);
                                r.RelativeItem().AlignRight().Text("HR Dept").FontSize(9);
                            });
                        });
                    });
                });
            });

            var pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Leave_Application_{app.EmployeeId}_{app.LeaveDate:yyyyMMdd}.pdf");
        }
    }

    public class ApproveRequest { public string ApprovedBy { get; set; } = "HR Manager"; }
    public class RejectRequest  { public string RejectedBy { get; set; } = "HR Manager"; public string Reason { get; set; } = ""; }
}
