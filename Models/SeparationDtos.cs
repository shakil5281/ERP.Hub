namespace ERPHub.Models;

public class RecordSeparationRequest
{
    public string EmployeeId { get; set; } = string.Empty;
    public string SeparationType { get; set; } = SeparationTypes.Left;
    public DateTime SeparationDate { get; set; } = DateTime.Today;
    public string Reason { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
    public DateTime? ApprovedDate { get; set; }
    public string CreatedBy { get; set; } = "System";
}

public class SeparationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Separation? Separation { get; set; }
}

public class SeparationFilter
{
    public string? SeparationType { get; set; }
    public int? CompanyId { get; set; }
    public int? DepartmentId { get; set; }
    public int? SectionId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool IncludeCancelled { get; set; }
}

public class EmployeeSeparationDto
{
    public int SeparationId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string SeparationType { get; set; } = string.Empty;
    public DateTime SeparationDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public bool IsSettled { get; set; }
    public DateTime CreatedDate { get; set; }
}
