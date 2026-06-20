using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPHub.Models
{
    [Table("manpowers")]
    public class Manpower
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }

        public int? SectionId { get; set; }

        public Section? Section { get; set; }

        public int? DesignationId { get; set; }

        public Designation? Designation { get; set; }

        [Required]
        public int TargetCapacity { get; set; }

        [Required]
        public int CurrentHeadcount { get; set; } = 0;

        [Required]
        public int Vacancies { get; set; } = 0;

        public string Remarks { get; set; } = string.Empty;

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}