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

        [StringLength(50)]
        public string OffDay { get; set; } = string.Empty;
    }
}