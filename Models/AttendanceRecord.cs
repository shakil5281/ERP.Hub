using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models
{
    [Table("attendancerecords")]
    public class AttendanceRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        public DateTime AttendanceDate { get; set; }

        public int ShiftId { get; set; }

        public DateTime? ActualInPunch { get; set; }

        public DateTime? ActualOutPunch { get; set; }

        public TimeSpan? AttendanceInTime { get; set; }

        public TimeSpan? AttendanceOutTime { get; set; }

        public int LateMinutes { get; set; }

        public int EarlyExitMinutes { get; set; }

        public int WorkedMinutes { get; set; }

        public int OvertimeMinutes { get; set; }

        [MaxLength(50)]
        public string AttendanceStatus { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Remarks { get; set; } = string.Empty;
    }
}
