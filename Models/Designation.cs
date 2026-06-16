using System.ComponentModel.DataAnnotations;

namespace ERPHub.Models
{
    public class Designation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Designation name (English) is required")]
        [StringLength(100, ErrorMessage = "Designation name is too long")]
        public string NameEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Designation name (Bengali) is required")]
        [StringLength(100, ErrorMessage = "Designation name is too long")]
        public string NameBn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Section is required")]
        public int SectionId { get; set; }

        public string SectionNameEn { get; set; } = string.Empty;
    }
}
