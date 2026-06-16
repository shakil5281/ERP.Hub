using ERPHub.Models;

namespace ERPHub.Services
{
    public interface IErpService
    {
        // Products (Inventory)
        Task<List<Product>> GetProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);

        // Invoices (Sales)
        Task<List<Invoice>> GetInvoicesAsync();
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task AddInvoiceAsync(Invoice invoice);
        Task UpdateInvoiceAsync(Invoice invoice);
        Task DeleteInvoiceAsync(int id);

        // Tasks (Kanban)
        Task<List<ProjectTask>> GetTasksAsync();
        Task<ProjectTask?> GetTaskByIdAsync(int id);
        Task AddTaskAsync(ProjectTask task);
        Task UpdateTaskAsync(ProjectTask task);
        Task UpdateTaskStatusAsync(int taskId, Models.TaskStatus status);
        Task DeleteTaskAsync(int id);

        // Companies
        Task<List<Company>> GetCompaniesAsync();
        Task<Company?> GetCompanyByIdAsync(int id);
        Task AddCompanyAsync(Company company);
        Task UpdateCompanyAsync(Company company);
        Task DeleteCompanyAsync(int id);

        // Dashboard/ERP Statistics
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalStockAsync();
        Task<int> GetPendingTasksCountAsync();
        Task<int> GetPaidInvoicesCountAsync();

        // Organogram Tree
        Task<List<CompanyNodeDto>> GetOrganogramTreeAsync();
    }
}
