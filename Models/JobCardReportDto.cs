namespace ERPHub.Models
{
    public class JobCardReportDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyAddress { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;

        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Grade { get; set; } = "n/a";
        public string Department { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
        public string JoiningDate { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;

        public List<JobCardDayRowDto> Days { get; set; } = new();
        public JobCardSummaryDto Summary { get; set; } = new();
        public JobCardTotalsDto Totals { get; set; } = new();
    }

    public class JobCardDayRowDto
    {
        public string Date { get; set; } = string.Empty;
        public string Shift { get; set; } = string.Empty;
        public string InTime { get; set; } = "00:00";
        public string OutTime { get; set; } = "00:00";
        public string Late { get; set; } = "00:00";
        public string WorkingHour { get; set; } = "0:00";
        public string EarlyOut { get; set; } = "0:00";
        public string Overtime { get; set; } = "0.00";
        public string Status { get; set; } = "A";
    }

    public class JobCardSummaryDto
    {
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
    }

    public class JobCardTotalsDto
    {
        public string Late { get; set; } = "00:00";
        public string EarlyOut { get; set; } = "0:00";
        public string Overtime { get; set; } = "0.00";
    }

    public class JobCardReportFilter
    {
        public string EmployeeId { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        public int? SectionId { get; set; }
        public int? DesignationId { get; set; }
        public int? LineId { get; set; }
        public int? ShiftId { get; set; }
    }

    public class JobCardReportPageDto
    {
        public JobCardReportDto Report { get; set; } = new();
        public int CurrentIndex { get; set; }
        public int TotalEmployees { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}
