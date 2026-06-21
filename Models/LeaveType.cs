using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models
{
    [Table("leavetypes")]
    public class LeaveType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        /// <summary>Max days allowed per calendar year</summary>
        public int MaxDaysPerYear { get; set; } = 10;

        public bool IsPaid { get; set; } = true;

        /// <summary>Monthly, Yearly, OnRequest</summary>
        [MaxLength(30)]
        public string AccrualType { get; set; } = "Yearly";

        public bool IsActive { get; set; } = true;

        /// <summary>Hex color for badge (e.g. #10b981)</summary>
        [MaxLength(10)]
        public string Color { get; set; } = "#6366f1";

        public bool RequiresMedicalCertificate { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
