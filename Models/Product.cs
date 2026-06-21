using System.ComponentModel.DataAnnotations;

namespace ERPHub.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name is too long")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU is required")]
        [RegularExpression(@"^[A-Z0-9-]{3,15}$", ErrorMessage = "SKU must be 3-15 alphanumeric uppercase characters or hyphens (e.g. LAP-001)")]
        public string Sku { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = "Hardware";

        [Range(0.01, 100000.0, ErrorMessage = "Price must be between 0.01 and 100,000.00")]
        public decimal Price { get; set; }

        [Range(0, 100000, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
