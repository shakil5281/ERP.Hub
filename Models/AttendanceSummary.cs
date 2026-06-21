namespace ERPHub.Models
{
    public record AttendanceSummary(
        int TotalEmployees,
        int TotalPresent,
        int TotalAbsent,
        int TotalLate,
        int TotalLeave
    );
}