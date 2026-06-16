using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPHub.Data;
using ERPHub.Models;

namespace ERPHub.Services
{
    public class ErpService : IErpService
    {
        private readonly ErpDbContext _context;

        public ErpService(ErpDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // --- Products Operations ---
        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.OrderByDescending(p => p.Id).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            product.Id = 0; // Reset ID to let SQL Server identity handle it
            product.LastUpdated = DateTime.Now;
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existing = await _context.Products.FindAsync(product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Sku = product.Sku;
                existing.Category = product.Category;
                existing.Price = product.Price;
                existing.Stock = product.Stock;
                existing.Description = product.Description;
                existing.LastUpdated = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing != null)
            {
                _context.Products.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        // --- Invoices Operations ---
        public async Task<List<Invoice>> GetInvoicesAsync()
        {
            return await _context.Invoices
                .Include(i => i.Items)
                .OrderByDescending(i => i.Id)
                .ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddInvoiceAsync(Invoice invoice)
        {
            invoice.Id = 0; // Let SQL Server identity handle it
            foreach (var item in invoice.Items)
            {
                item.Id = 0;
            }
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInvoiceAsync(Invoice invoice)
        {
            var existing = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == invoice.Id);

            if (existing != null)
            {
                existing.InvoiceNumber = invoice.InvoiceNumber;
                existing.ClientName = invoice.ClientName;
                existing.ClientEmail = invoice.ClientEmail;
                existing.Status = invoice.Status;
                existing.DueDate = invoice.DueDate;
                existing.TaxRate = invoice.TaxRate;
                existing.DiscountAmount = invoice.DiscountAmount;

                // Remove existing items from database context
                _context.InvoiceItems.RemoveRange(existing.Items);
                existing.Items.Clear();

                // Add the updated items with reset IDs to avoid constraint issues
                foreach (var item in invoice.Items)
                {
                    item.Id = 0;
                    existing.Items.Add(item);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteInvoiceAsync(int id)
        {
            var existing = await _context.Invoices.FindAsync(id);
            if (existing != null)
            {
                _context.Invoices.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        // --- Tasks Operations ---
        public async Task<List<ProjectTask>> GetTasksAsync()
        {
            return await _context.ProjectTasks.OrderByDescending(t => t.Id).ToListAsync();
        }

        public async Task<ProjectTask?> GetTaskByIdAsync(int id)
        {
            return await _context.ProjectTasks.FindAsync(id);
        }

        public async Task AddTaskAsync(ProjectTask task)
        {
            task.Id = 0; // Let SQL Server identity handle it
            task.CreatedAt = DateTime.Now;
            await _context.ProjectTasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(ProjectTask task)
        {
            var existing = await _context.ProjectTasks.FindAsync(task.Id);
            if (existing != null)
            {
                existing.Title = task.Title;
                existing.Description = task.Description;
                existing.Status = task.Status;
                existing.Priority = task.Priority;
                existing.Assignee = task.Assignee;
                existing.DueDate = task.DueDate;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateTaskStatusAsync(int taskId, Models.TaskStatus status)
        {
            var existing = await _context.ProjectTasks.FindAsync(taskId);
            if (existing != null)
            {
                existing.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteTaskAsync(int id)
        {
            var existing = await _context.ProjectTasks.FindAsync(id);
            if (existing != null)
            {
                _context.ProjectTasks.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        // --- Companies Operations ---
        public async Task<List<Company>> GetCompaniesAsync()
        {
            return await _context.Companies.OrderByDescending(c => c.Id).ToListAsync();
        }

        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            return await _context.Companies.FindAsync(id);
        }

        public async Task AddCompanyAsync(Company company)
        {
            company.Id = 0; // Let SQL Server identity handle it
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            var existing = await _context.Companies.FindAsync(company.Id);
            if (existing != null)
            {
                existing.CompanyNameEn = company.CompanyNameEn;
                existing.CompanyNameBn = company.CompanyNameBn;
                existing.AddressEn = company.AddressEn;
                existing.AddressBn = company.AddressBn;
                existing.Email = company.Email;
                existing.PhoneNumber = company.PhoneNumber;
                existing.Signature = company.Signature;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCompanyAsync(int id)
        {
            var existing = await _context.Companies.FindAsync(id);
            if (existing != null)
            {
                _context.Companies.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        // --- Dashboard / Statistics ---
        public async Task<decimal> GetTotalRevenueAsync()
        {
            var paidInvoices = await _context.Invoices
                .Include(i => i.Items)
                .Where(i => i.Status == InvoiceStatus.Paid)
                .ToListAsync();

            return paidInvoices.Sum(i => i.GrandTotal);
        }

        public async Task<int> GetTotalStockAsync()
        {
            return await _context.Products.SumAsync(p => p.Stock);
        }

        public async Task<int> GetPendingTasksCountAsync()
        {
            return await _context.ProjectTasks.CountAsync(t => t.Status != Models.TaskStatus.Done);
        }

        public async Task<int> GetPaidInvoicesCountAsync()
        {
            return await _context.Invoices.CountAsync(i => i.Status == InvoiceStatus.Paid);
        }

        // --- Organogram Tree ---
        public async Task<List<CompanyNodeDto>> GetOrganogramTreeAsync()
        {
            var companies = await _context.Companies.ToListAsync();
            var departments = await _context.Departments.ToListAsync();
            var sections = await _context.Sections.ToListAsync();
            var designations = await _context.Designations.ToListAsync();
            var lines = await _context.Lines.ToListAsync();

            var tree = new List<CompanyNodeDto>();

            // For now, attach all departments to all companies, 
            // since there is no direct Company-Department relationship in the DB.
            foreach (var company in companies)
            {
                var companyNode = new CompanyNodeDto
                {
                    Id = company.Id,
                    NameEn = company.CompanyNameEn,
                    NameBn = company.CompanyNameBn,
                    Departments = departments.Select(d => new DepartmentNodeDto
                    {
                        Id = d.Id,
                        NameEn = d.NameEn,
                        NameBn = d.NameBn,
                        Sections = sections.Where(s => s.DepartmentId == d.Id).Select(s => new SectionNodeDto
                        {
                            Id = s.Id,
                            NameEn = s.NameEn,
                            NameBn = s.NameBn,
                            Designations = designations.Where(deg => deg.SectionId == s.Id).Select(deg => new DesignationNodeDto
                            {
                                Id = deg.Id,
                                NameEn = deg.NameEn,
                                NameBn = deg.NameBn
                            }).ToList(),
                            Lines = lines.Where(l => l.SectionId == s.Id).Select(l => new LineNodeDto
                            {
                                Id = l.Id,
                                NameEn = l.NameEn,
                                NameBn = l.NameBn
                            }).ToList()
                        }).ToList()
                    }).ToList()
                };
                tree.Add(companyNode);
            }

            return tree;
        }
    }
}
