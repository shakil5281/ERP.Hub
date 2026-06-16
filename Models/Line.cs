using System.ComponentModel.DataAnnotations;

namespace ERPHub.Models
{
    public class Line
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Line name (English) is required")]
        [StringLength(100, ErrorMessage = "Line name is too long")]
        public string NameEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Line name (Bengali) is required")]
        [StringLength(100, ErrorMessage = "Line name is too long")]
        public string NameBn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Section is required")]
        public int SectionId { get; set; }

        public string SectionNameEn { get; set; } = string.Empty;
    }
}
