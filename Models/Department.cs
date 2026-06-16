using System.ComponentModel.DataAnnotations;

namespace ERPHub.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name (English) is required")]
        [StringLength(100, ErrorMessage = "Department name is too long")]
        public string NameEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department name (Bengali) is required")]
        [StringLength(100, ErrorMessage = "Department name is too long")]
        public string NameBn { get; set; } = string.Empty;
    }
}
