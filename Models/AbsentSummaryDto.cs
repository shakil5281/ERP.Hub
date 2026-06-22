namespace ERPHub.Models
{
    public class AbsentSummaryDto
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Shift { get; set; } = string.Empty;
        public int TotalAbsentDays { get; set; }
        public int ContinuousAbsentDays { get; set; }
    }
}
