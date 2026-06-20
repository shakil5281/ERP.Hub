using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models
{
    [Table("manpowerrequirements")]
    public class ManpowerRequirement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string RoleTitle { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }

        public int? SectionId { get; set; }

        public Section? Section { get; set; }

        public int? DesignationId { get; set; }

        public Designation? Designation { get; set; }

        [Required]
        public int HeadcountNeeded { get; set; } = 1;

        [Required]
        public DateTime TargetDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Priority { get; set; } = "Medium";

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string Requirements { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
    }
}