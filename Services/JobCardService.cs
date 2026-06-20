using ERPHub.Data;
using ERPHub.Models;
using Microsoft.EntityFrameworkCore;

namespace ERPHub.Services
{
    public class JobCardService
    {
        private readonly ErpDbContext _context;

        public JobCardService(ErpDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<string>> GetFilteredEmployeeIdsAsync(JobCardReportFilter filter)
        {
            return await ApplyEmployeeFilters(filter)
                .OrderBy(e => e.EmployeeId)
                .Select(e => e.EmployeeId)
                .ToListAsync();
        }

        public async Task<JobCardReportPageDto?> GetJobCardReportPageAsync(JobCardReportFilter filter, int index = 0)
        {
            var employeeIds = await GetFilteredEmployeeIdsAsync(filter);
            if (employeeIds.Count == 0)
                return null;

            int actualIndex;
            if (!string.IsNullOrWhiteSpace(filter.EmployeeId))
            {
                var matchIndex = employeeIds.FindIndex(id =>
                    id.Equals(filter.EmployeeId.Trim(), StringComparison.OrdinalIgnoreCase));
                if (matchIndex < 0)
                    return null;
                actualIndex = matchIndex;
            }
            else
            {
                actualIndex = Math.Clamp(index, 0, employeeIds.Count - 1);
            }

            var employeeFilter = new JobCardReportFilter
            {
                EmployeeId = employeeIds[actualIndex],
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                CompanyId = filter.CompanyId,
                DepartmentId = filter.DepartmentId,
                SectionId = filter.SectionId,
                DesignationId = filter.DesignationId,
                LineId = filter.LineId,
                ShiftId = filter.ShiftId
            };

            var report = await GetJobCardReportAsync(employeeFilter);
            if (report == null)
                return null;

            return new JobCardReportPageDto
            {
                Report = report,
                CurrentIndex = actualIndex,
                TotalEmployees = employeeIds.Count,
                HasPrevious = actualIndex > 0,
                HasNext = actualIndex < employeeIds.Count - 1
            };
        }

        public async Task<JobCardReportDto?> GetJobCardReportAsync(JobCardReportFilter filter)
        {
            if (string.IsNullOrWhiteSpace(filter.EmployeeId))
                return null;

            var from = filter.FromDate.Date;
            var to = filter.ToDate.Date;
            if (to < from)
                (from, to) = (to, from);

            var employeeQuery = ApplyEmployeeFilters(filter)
                .Include(e => e.Department)
                .Include(e => e.Section)
                .Include(e => e.Designation)
                .Include(e => e.Line)
                .Include(e => e.Shift);

            var employee = await employeeQuery.FirstOrDefaultAsync();
            if (employee == null)
                return null;

            Company? company = null;
            if (filter.CompanyId > 0)
                company = await _context.Companies.FindAsync(filter.CompanyId);
            company ??= await _context.Companies.OrderBy(c => c.Id).FirstOrDefaultAsync();

            var attendanceRecords = await _context.AttendanceRecords
                .Where(a => a.EmployeeId == employee.EmployeeId && a.AttendanceDate >= from && a.AttendanceDate <= to)
                .ToListAsync();

            var attendanceByDate = attendanceRecords.ToDictionary(a => a.AttendanceDate.Date);
            var holidays = await _context.Holidays
                .Where(h => h.HolidayDate >= from && h.HolidayDate <= to)
                .Select(h => h.HolidayDate.Date)
                .ToHashSetAsync();

            var shiftMap = await _context.Shifts.ToDictionaryAsync(s => s.Id);
            var days = new List<JobCardDayRowDto>();
            var summary = new JobCardSummaryDto();
            var totalLateMinutes = 0;
            var totalEarlyOutMinutes = 0;
            var totalOvertimeMinutes = 0;

            for (var date = from; date <= to; date = date.AddDays(1))
            {
                attendanceByDate.TryGetValue(date, out var record);
                var shift = record != null && shiftMap.TryGetValue(record.ShiftId, out var recordShift)
                    ? recordShift
                    : employee.Shift;

                var row = BuildDayRow(date, record, shift, holidays);
                days.Add(row);

                if (row.Status == "P")
                {
                    summary.Present++;
                    if (record?.LateMinutes > 0)
                        summary.Late++;
                }
                else if (row.Status == "A")
                {
                    summary.Absent++;
                }

                if (record != null)
                {
                    totalLateMinutes += record.LateMinutes;
                    totalEarlyOutMinutes += record.EarlyExitMinutes;
                    totalOvertimeMinutes += record.OvertimeMinutes;
                }
            }

            return new JobCardReportDto
            {
                CompanyName = company?.CompanyNameEn ?? "Company",
                CompanyAddress = company?.AddressEn ?? string.Empty,
                FromDate = from.ToString("dd/MM/yyyy"),
                ToDate = to.ToString("dd/MM/yyyy"),
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.EmployeeName,
                Designation = employee.Designation?.NameEn ?? string.Empty,
                Department = employee.Department?.NameEn ?? string.Empty,
                Line = employee.Line?.NameEn ?? string.Empty,
                JoiningDate = employee.JoiningDate.ToString("dd/MM/yyyy"),
                Section = employee.Section?.NameEn ?? string.Empty,
                Days = days,
                Summary = summary,
                Totals = new JobCardTotalsDto
                {
                    Late = FormatMinutesAsHhMm(totalLateMinutes),
                    EarlyOut = FormatMinutesAsHM(totalEarlyOutMinutes),
                    Overtime = FormatOvertimeHours(totalOvertimeMinutes)
                }
            };
        }

        private static JobCardDayRowDto BuildDayRow(DateTime date, AttendanceRecord? record, Shift? shift, HashSet<DateTime> holidays)
        {
            var isOffDay = shift != null &&
                !string.IsNullOrEmpty(shift.OffDay) &&
                shift.OffDay.Contains(date.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase);
            var isHoliday = holidays.Contains(date.Date);

            if (record == null)
            {
                var status = isOffDay || isHoliday ? "W" : "A";
                return new JobCardDayRowDto
                {
                    Date = date.ToString("dd/MM/yyyy"),
                    Shift = shift?.ShiftName ?? string.Empty,
                    Status = status
                };
            }

            var statusCode = MapStatusCode(record.AttendanceStatus, isOffDay, isHoliday);

            return new JobCardDayRowDto
            {
                Date = date.ToString("dd/MM/yyyy"),
                Shift = shift?.ShiftName ?? string.Empty,
                InTime = FormatPunchTime(record.ActualInPunch),
                OutTime = FormatPunchTime(record.ActualOutPunch),
                Late = FormatMinutesAsHhMm(record.LateMinutes),
                WorkingHour = FormatMinutesAsHM(record.WorkedMinutes),
                EarlyOut = FormatMinutesAsHM(record.EarlyExitMinutes),
                Overtime = FormatOvertimeHours(record.OvertimeMinutes),
                Status = statusCode
            };
        }

        private static string MapStatusCode(string attendanceStatus, bool isOffDay, bool isHoliday)
        {
            if (attendanceStatus is "Weekly Off" or "Holiday" or "Weekly Off Worked" or "Holiday Worked")
                return "W";
            if (attendanceStatus == "Absent" || attendanceStatus == "Leave")
                return "A";
            if (isOffDay || isHoliday)
                return "W";
            return "P";
        }

        private static string FormatPunchTime(DateTime? value)
            => value?.ToString("HH:mm") ?? "00:00";

        private static string FormatMinutesAsHhMm(int minutes)
            => TimeSpan.FromMinutes(Math.Max(0, minutes)).ToString(@"hh\:mm");

        private static string FormatMinutesAsHM(int minutes)
            => TimeSpan.FromMinutes(Math.Max(0, minutes)).ToString(@"h\:mm");

        private static string FormatOvertimeHours(int minutes)
            => (Math.Max(0, minutes) / 60.0).ToString("0.00");

        private IQueryable<Employee> ApplyEmployeeFilters(JobCardReportFilter filter)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.EmployeeId))
                query = query.Where(e => e.EmployeeId == filter.EmployeeId.Trim());

            if (filter.DepartmentId > 0)
                query = query.Where(e => e.DepartmentId == filter.DepartmentId);
            if (filter.SectionId > 0)
                query = query.Where(e => e.SectionId == filter.SectionId);
            if (filter.DesignationId > 0)
                query = query.Where(e => e.DesignationId == filter.DesignationId);
            if (filter.LineId > 0)
                query = query.Where(e => e.LineId == filter.LineId);
            if (filter.ShiftId > 0)
                query = query.Where(e => e.ShiftId == filter.ShiftId);

            return query;
        }
    }
}
