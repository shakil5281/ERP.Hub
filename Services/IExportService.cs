using System.Collections.Generic;
using System.Threading.Tasks;
using ERPHub.Models;

namespace ERPHub.Services
{
    public interface IExportService
    {
        Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, string title, Dictionary<string, System.Func<T, object>> columns);
        Task<byte[]> ExportToPdfAsync<T>(IEnumerable<T> data, string title, Dictionary<string, System.Func<T, object>> columns);
        Task<byte[]> ExportAttendanceToExcelAsync(
            IEnumerable<AttendanceRecord> data,
            string reportTitle,
            DateTime reportDate,
            Company company,
            AttendanceSummary summary,
            Dictionary<string, Func<AttendanceRecord, object>> columns);
        Task<byte[]> ExportAttendanceToPdfAsync(
            IEnumerable<AttendanceRecord> data,
            string reportTitle,
            DateTime reportDate,
            Company company,
            AttendanceSummary summary,
            Dictionary<string, Func<AttendanceRecord, object>> columns);
        Task<byte[]> ExportJobCardToExcelAsync(JobCardReportDto report);
        Task<byte[]> ExportJobCardToPdfAsync(JobCardReportDto report);
    }
}