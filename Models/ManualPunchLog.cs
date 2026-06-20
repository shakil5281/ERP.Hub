using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models
{
    [Table("manualpunchlogs")]
    public class ManualPunchLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        public DateTime PunchDate { get; set; }

        public TimeSpan PunchTime { get; set; }

        [Required]
        [MaxLength(50)]
        public string PunchType { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Reason { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";
    }
}
