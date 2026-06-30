using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ERPHub.Models
{
    public class Section
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Section name (English) is required")]
        [StringLength(100, ErrorMessage = "Section name is too long")]
        public string NameEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Section name (Bengali) is required")]
        [StringLength(100, ErrorMessage = "Section name is too long")]
        public string NameBn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required")]
        public int DepartmentId { get; set; }

        public Department? Department { get; set; }

        public string DepartmentNameEn { get; set; } = string.Empty;

        public ICollection<Designation> Designations { get; set; } = new List<Designation>();
        public ICollection<Line> Lines { get; set; } = new List<Line>();
    }
}
