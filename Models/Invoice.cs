using System.ComponentModel.DataAnnotations;

namespace ERPHub.Models
{
    public enum InvoiceStatus
    {
        Draft,
        Sent,
        Paid,
        Overdue,
        Cancelled
    }

    public class InvoiceItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Item description is required")]
        public string Description { get; set; } = string.Empty;

        [Range(1, 10000, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;

        [Range(0.01, 100000.0, ErrorMessage = "Unit price must be positive")]
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;
    }

    public class Invoice
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Invoice number is required")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client name is required")]
        public string ClientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string ClientEmail { get; set; } = string.Empty;

        public DateTime IssueDate { get; set; } = DateTime.Now;

        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(30);

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

        public List<InvoiceItem> Items { get; set; } = new();

        [Range(0, 100, ErrorMessage = "Tax rate must be between 0% and 100%")]
        public decimal TaxRate { get; set; } = 0.15m; // 15% default

        [Range(0, 1000000, ErrorMessage = "Discount cannot be negative")]
        public decimal DiscountAmount { get; set; } = 0.00m;

        public decimal SubTotal => Items.Sum(i => i.TotalPrice);

        public decimal TaxAmount => SubTotal * TaxRate;

        public decimal GrandTotal => SubTotal + TaxAmount - DiscountAmount;
    }
}
