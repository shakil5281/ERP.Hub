using System;
using System.Linq;
using System.Threading.Tasks;
using ERPHub.Data;
using ERPHub.Models;
using ERPHub.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERPHub.Jobs
{
    public class DailyAttendanceProcessor
    {
        private readonly IDbContextFactory<ErpDbContext> _dbFactory;
        private readonly ILogger<DailyAttendanceProcessor> _logger;

        public DailyAttendanceProcessor(IDbContextFactory<ErpDbContext> dbFactory, ILogger<DailyAttendanceProcessor> logger)
        {
            _dbFactory = dbFactory;
            _logger = logger;
        }

        // Hangfire entry point
        public async Task ProcessYesterdayAttendanceAsync()
        {
            // Usually we process the previous day
            var targetDate = DateTime.Today.AddDays(-1);
            await ProcessAttendanceForDateAsync(targetDate);
        }

        public async Task ProcessAttendanceForDateAsync(DateTime date)
        {
            _logger.LogInformation($"Starting attendance processing for {date:yyyy-MM-dd}");

            await using var db = await _dbFactory.CreateDbContextAsync();

            var punches = await db.PunchRecords
                .Where(p => p.LogDateTime.Date == date.Date)
                .ToListAsync();

            var employees = await db.Employees
                .Include(e => e.Shift)
                .EligibleOnDate(date)
                .ToListAsync();

            var leaveApplications = await db.LeaveApplications
                .Include(la => la.LeaveTypeNav)
                .Where(la => la.Status == "Approved" && la.LeaveDate.Date == date.Date)
                .ToListAsync();

            // Group punches by PunchNumber
            var groupedPunches = punches.GroupBy(p => p.PunchNumber).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var employee in employees)
            {
                if (employee.PunchNumber <= 0) continue;

                int empPunchNumber = employee.PunchNumber;

                var existingRecord = await db.AttendanceRecords
                    .FirstOrDefaultAsync(a => a.EmployeeId == employee.EmployeeId && a.AttendanceDate == date.Date);

                var record = existingRecord ?? new AttendanceRecord
                {
                    EmployeeId = employee.EmployeeId,
                    AttendanceDate = date.Date,
                    ShiftId = employee.ShiftId
                };

                bool hasPunches = groupedPunches.TryGetValue(empPunchNumber, out var empPunches);

                if (hasPunches && empPunches!.Any())
                {
                    var sortedPunches = empPunches!.OrderBy(p => p.LogDateTime).ToList();
                    var firstPunch = sortedPunches.First().LogDateTime;
                    var lastPunch = sortedPunches.Last().LogDateTime;

                    record.ActualInPunch = firstPunch;
                    record.AttendanceInTime = firstPunch.TimeOfDay;

                    if (sortedPunches.Count > 1 && (lastPunch - firstPunch).TotalMinutes > employee.Shift?.DuplicateIntervalMinutes)
                    {
                        record.ActualOutPunch = lastPunch;
                        record.AttendanceOutTime = lastPunch.TimeOfDay;
                        record.AttendanceStatus = "Present";

                        // Calculate Worked Minutes
                        record.WorkedMinutes = (int)(lastPunch - firstPunch).TotalMinutes;

                        // Shift Calculations
                        if (employee.Shift != null)
                        {
                            // Late Calculation
                            var expectedIn = employee.Shift.InTime;
                            var actualIn = record.AttendanceInTime.Value;
                            var lateDiff = (actualIn - expectedIn).TotalMinutes;

                            if (lateDiff > employee.Shift.GraceInMinutes)
                            {
                                record.LateMinutes = (int)lateDiff;
                            }
                            else
                            {
                                record.LateMinutes = 0;
                            }

                            // Early Exit Calculation
                            var expectedOut = employee.Shift.OutTime;
                            var actualOut = record.AttendanceOutTime.Value;
                            var earlyDiff = (expectedOut - actualOut).TotalMinutes;

                            if (earlyDiff > 0)
                            {
                                record.EarlyExitMinutes = (int)earlyDiff;
                            }
                            else
                            {
                                record.EarlyExitMinutes = 0;
                            }

                            // Overtime Calculation
                            if (employee.OverTimeStatus)
                            {
                                var otDiff = (actualOut - expectedOut).TotalMinutes;
                                if (otDiff >= employee.Shift.MinimumOvertimeMinutes)
                                {
                                    record.OvertimeMinutes = (int)otDiff;
                                }
                                else
                                {
                                    record.OvertimeMinutes = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        record.AttendanceStatus = "Missing Punch";
                        record.Remarks = "Only one punch recorded for the day.";
                    }
                }
                else
                {
                    // No punches - Check for Leave
                    var leave = leaveApplications.FirstOrDefault(la => la.EmployeeId == employee.EmployeeId);

                    if (leave != null)
                    {
                        record.AttendanceStatus = leave.LeaveTypeNav?.Name ?? "Leave";
                        record.Remarks = $"Approved Leave: {leave.Reason}";
                    }
                    else
                    {
                        record.AttendanceStatus = "Absent";
                        record.Remarks = "No punches found.";
                    }
                }

                if (existingRecord == null)
                {
                    db.AttendanceRecords.Add(record);
                }
                else
                {
                    db.AttendanceRecords.Update(record);
                }
            }

            await db.SaveChangesAsync();
            _logger.LogInformation($"Successfully processed attendance for {date:yyyy-MM-dd}. Records saved: {employees.Count}");
        }
    }
}
