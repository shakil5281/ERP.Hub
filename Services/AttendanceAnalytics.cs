namespace ERPHub.Services
{
    public static class AttendanceAnalytics
    {
        public static bool IsPresent(string? status) => status is
            "Present" or "Present + Overtime" or "Late" or "Early Exit" or
            "Half Day" or "Holiday Worked" or "Weekly Off Worked" or
            "Missing In Punch" or "Missing Out Punch";

        public static bool IsLate(string? status) => status is "Late" or "Early Exit";

        public static bool IsAbsent(string? status) =>
            status == "Absent" || string.IsNullOrEmpty(status);

        public static bool IsLeave(string? status) =>
            status is "Leave" or "Holiday" or "Weekly Off";

        public static bool IsPayableAbsence(string? status) => status is
            "Absent" or "Missing In Punch" or "Missing Out Punch";

        public static string GetShortStatus(string? status)
        {
            return status switch
            {
                "Present" or "Present + Overtime" => "P",
                "Late" => "L",
                "Early Exit" => "EE",
                "Half Day" => "HD",
                "Leave" => "LV",
                "Holiday" => "H",
                "Weekly Off" => "W",
                "Holiday Worked" => "HW",
                "Weekly Off Worked" => "WW",
                "Missing In Punch" => "MP",
                "Missing Out Punch" => "MO",
                "Absent" or null or "" => "A",
                _ => "A"
            };
        }

        public static string GetBadgeColor(string? status)
        {
            return status switch
            {
                "Present" or "Present + Overtime" => "green",
                "Late" or "Early Exit" or "Half Day" => "yellow",
                "Absent" or null or "" or "Missing In Punch" or "Missing Out Punch" => "red",
                "Leave" => "purple",
                "Holiday" or "Weekly Off" => "blue",
                "Holiday Worked" or "Weekly Off Worked" => "teal",
                _ => "default"
            };
        }
    }
}
