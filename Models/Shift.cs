using System;
using System.ComponentModel.DataAnnotations;

namespace ERPHub.Models
{
    public class Shift
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Shift Name is required")]
        [StringLength(100)]
        public string ShiftName { get; set; } = string.Empty;

        [Required]
        public TimeSpan InTime { get; set; }

        [Required]
        public TimeSpan OutTime { get; set; }

        public TimeSpan LateTime { get; set; }

        public int GraceInMinutes { get; set; } = 0;

        public int BreakMinutes { get; set; } = 30;

        public int HalfDayThresholdMinutes { get; set; } = 240;

        public int MinimumOvertimeMinutes { get; set; } = 30;

        public int DuplicateIntervalMinutes { get; set; } = 1;

        [StringLength(50)]
        public string OffDay { get; set; } = string.Empty;
    }
}